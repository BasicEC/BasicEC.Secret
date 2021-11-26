using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BasicEC.Secret.Extensions;
using BasicEC.Secret.ProgressBar;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BasicEC.Secret
{
    public class StreamDataConveyor : IProgressStatusProvider
    {
        private const int MaxBatchSize = 1024;

        private readonly int _threads;
        private readonly int _batchSize;
        private readonly Stream _source;
        private readonly Stream _destination;
        private readonly Func<byte[], byte[]> _processor;
        private readonly BehaviorSubject<ProgressStatus> _processStatus;
        private readonly ILogger _logger;

        public readonly long TotalBatches;

        public StreamDataConveyor(Stream source,
                                  Stream destination,
                                  int batchSize,
                                  Func<byte[], byte[]> processor,
                                  int threads = 1,
                                  ILogger logger = null)
        {
            if (batchSize is > MaxBatchSize or <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), $"Should be between 1 and {MaxBatchSize}");
            }

            if (threads <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(threads), "Should greater than 0");
            }

            _source = source ?? throw new ArgumentNullException(nameof(source));
            _destination = destination ?? throw new ArgumentNullException(nameof(destination));
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));

            _threads = threads;
            _batchSize = batchSize;
            TotalBatches = Math.DivRem(_source.Length, batchSize, out var rem) + Math.Sign(rem);
            _processStatus = new BehaviorSubject<ProgressStatus>(new ProgressStatus
            {
                ProcessedTasks = 0,
                TotalTasks = TotalBatches
            });
            _logger = logger ?? NullLogger.Instance;
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public async Task ProcessDataAsync()
        {
            using var writingQueue =
                new ProcessingQueue<DataEvent>("WritingQueue", CreateWriteProcessor(), logger: _logger);
            using var processingQueue =
                new ProcessingQueue<DataEvent>("ProcessingQueue", _ => ProcessData(writingQueue, _), _threads, _logger);

            await ReadAsync(processingQueue);

            _logger.LogTrace("Waiting for the end of the processing");
            processingQueue.WaitUntilProcessingFinished();
            writingQueue.WaitUntilProcessingFinished();
        }

        private void ProcessData(ProcessingQueue<DataEvent> destination, DataEvent @event)
        {
            destination.Enqueue(new DataEvent { Seq = @event.Seq, Data = _processor.Invoke(@event.Data) });
        }

        private Func<DataEvent, Task> CreateWriteProcessor()
        {
            var writeEvents = new SortedList<int, byte[]>();
            var cursor = 0;
            var processedBatches = 0;
            return async @event =>
            {
                _processStatus.OnNext(new ProgressStatus
                {
                    ProcessedTasks = ++processedBatches,
                    TotalTasks = TotalBatches
                });

                writeEvents.Add(@event.Seq, @event.Data);

                if (@event.Seq != cursor)
                {
                    return;
                }

                var count = 0;
                var first = writeEvents.First().Key;
                var readyToWrite = writeEvents.TakeWhile(_ => _.Key - first == count++).ToList();
                await _destination.WriteAsync(readyToWrite.SelectMany(_ => _.Value).ToArray());
                cursor = readyToWrite.Last().Key + 1;
                foreach (var pair in readyToWrite)
                {
                    writeEvents.Remove(pair.Key);
                }

                if (processedBatches == TotalBatches)
                {
                    _processStatus.OnCompleted();
                }
            };
        }

        private async Task ReadAsync(ProcessingQueue<DataEvent> destination)
        {
            var bufferSize = MaxBatchSize / _batchSize * _batchSize;

            int read, seq = 0;
            var buffer = new byte[bufferSize];
            var events = new List<DataEvent>();
            do
            {
                read = await _source.ReadAsync(buffer);
                _logger.LogTrace("Read batch of data. Size: {Size}", read);

                if (read < bufferSize)
                {
                    var buff = new byte[read];
                    Array.Copy(buffer, buff, read);
                    buffer = buff;
                }

                events.Clear();
                events.AddRange(buffer.Chunks(_batchSize).Select(_ => new DataEvent { Data = _, Seq = seq++ }));

                foreach (var @event in events)
                {
                    destination.Enqueue(@event);
                }
            } while (read > 0);
        }

        public IDisposable SubscribeOnProgressStatus(IObserver<ProgressStatus> observer)
        {
            return _processStatus.Subscribe(observer);
        }

        private class DataEvent
        {
            public byte[] Data { get; init; }
            public int Seq { get; init; }
        }
    }
}
