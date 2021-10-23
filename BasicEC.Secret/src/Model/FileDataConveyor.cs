using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BasicEC.Secret.Model.Extensions;
using Serilog;

namespace BasicEC.Secret.Model
{
    public class FileDataConveyor
    {
        private const int MaxBatchSize = 2048;

        private readonly int _threads;
        private readonly int _batchSize;
        private readonly FileInfo _source;
        private readonly FileInfo _destination;
        private readonly Func<byte[], byte[]> _processor;

        public FileDataConveyor(string source,
                                string destination,
                                int batchSize,
                                Func<byte[], byte[]> processor,
                                int threads = 1)
        {
            if (batchSize > MaxBatchSize)
                throw new ArgumentOutOfRangeException(nameof(batchSize), $"Should be between 0 and {MaxBatchSize}");

            _threads = threads;
            _batchSize = batchSize;
            _source = source.CheckFileExists();
            _destination = new FileInfo(destination);
            _processor = processor;
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public async Task ProcessDataAsync()
        {
            await using var writeStream = _destination.Open(FileMode.Create, FileAccess.Write);

            using var writingQueue = new ProcessingQueue<DataEvent>("WritingQueue", CreateWriteProcessor(writeStream));
            using var processingQueue =
                new ProcessingQueue<DataEvent>("ProcessingQueue", _ => ProcessData(writingQueue, _), _threads);

            await ReadFileAsync(processingQueue);

            Log.Logger.Verbose("Waiting for the end of the processing");
            processingQueue.WaitUntilProcessingFinished();
            writingQueue.WaitUntilProcessingFinished();
        }

        private void ProcessData(ProcessingQueue<DataEvent> destination, DataEvent @event)
        {
            destination.Enqueue(new DataEvent { Seq = @event.Seq, Data = _processor.Invoke(@event.Data) });
        }

        private static Func<DataEvent, Task> CreateWriteProcessor(Stream fs)
        {
            var writeEvents = new SortedList<int, byte[]>();
            var cursor = 0;
            return async @event =>
            {
                writeEvents.Add(@event.Seq, @event.Data);

                if (@event.Seq != cursor)
                {
                    return;
                }

                var count = 0;
                var first = writeEvents.First().Key;
                var readyToWrite = writeEvents.TakeWhile(_ => _.Key - first == count++).ToList();
                await fs.WriteAsync(readyToWrite.SelectMany(_ => _.Value).ToArray());
                cursor = readyToWrite.Last().Key + 1;
                foreach (var pair in readyToWrite)
                {
                    writeEvents.Remove(pair.Key);
                }
            };
        }

        private async Task ReadFileAsync(ProcessingQueue<DataEvent> destination)
        {
            await using var readStream = _source.OpenRead();
            var bufferSize = MaxBatchSize / _batchSize * _batchSize;

            int read, seq = 0;
            var buffer = new byte[bufferSize];
            var events = new List<DataEvent>();
            do
            {
                read = await readStream.ReadAsync(buffer);
                Log.Logger.Verbose("Read batch of data. Size: {Size}", read);

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

        private class DataEvent
        {
            public byte[] Data { get; init; }
            public int Seq { get; set; }
        }
    }
}
