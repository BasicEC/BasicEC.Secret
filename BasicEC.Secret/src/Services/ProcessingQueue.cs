using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace BasicEC.Secret.Services
{
    public class ProcessingQueue<T> : IDisposable
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly ManualResetEvent _enqueueEvent;
        private readonly ManualResetEvent _processingFinishedEvent;
        private readonly CancellationTokenSource _cts;
        private readonly Task[] _jobs;

        private int _threads;

        public readonly string Name;

        public ProcessingQueue(string name, Action<T> processor, int threads = 1)
            : this(name, async data => processor(data), threads)
        {
        }

        public ProcessingQueue(string name, Func<T, Task> processor, int threads = 1)
        {
            Name = name;
            _queue = new ConcurrentQueue<T>();
            _enqueueEvent = new ManualResetEvent(false);
            _processingFinishedEvent = new ManualResetEvent(true);
            _cts = new CancellationTokenSource();
            _jobs = Enumerable.Range(0, threads)
                .Select(_ => Task.Run(async () => await ProcessAsync(processor, _), _cts.Token)).ToArray();
        }

        private async Task ProcessAsync(Func<T, Task> processor, int thread)
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    _enqueueEvent.Reset();
                    _processingFinishedEvent.Reset();
                    Interlocked.Increment(ref _threads);
                    while (_queue.TryDequeue(out var @event))
                    {
                        try
                        {
                            await processor.Invoke(@event);
                        }
                        catch (Exception e)
                        {
                            Log.Logger.Warning(e, "Unexpected error during queue processing");
                        }
                    }

                    var workers = Interlocked.Decrement(ref _threads);
                    if (workers == 0)
                    {
                        _processingFinishedEvent.Set();
                    }

                    Log.Logger.Verbose("{Name} {Thread} All available data is processed", Name, thread);
                    _enqueueEvent.WaitOne();
                }
                catch (OperationCanceledException e) when (e.CancellationToken.Equals(_cts.Token))
                {
                    Log.Logger.Verbose("{Name} {Thread} Processing is cancelled", Name, thread);
                    break;
                }
            }
        }

        public void Enqueue(T data)
        {
            _queue.Enqueue(data);
            _enqueueEvent.Set();
        }

        public void WaitUntilProcessingFinished()
        {
            _processingFinishedEvent.WaitOne();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _enqueueEvent.Set();
            Task.WaitAll(_jobs);
            _cts.Dispose();
            _enqueueEvent.Dispose();
            _processingFinishedEvent.Dispose();
        }
    }
}
