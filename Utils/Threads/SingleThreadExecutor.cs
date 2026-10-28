using System;
using System.Collections.Concurrent;
using System.Threading;
using FuturesV2._Base;

namespace OEPFrameworkV3.Utils.Threads
{
    public class SingleThreadExecutor : IExecutor
    {
        public bool IsAlive => !_lock;
        public int TaskCount => _tasks.Count;
        private readonly BlockingCollection<IFuture> _tasks = new();
        private readonly CancellationTokenSource _source = new();        
        private readonly Thread _thread;
        private volatile bool _lock;
        private readonly object _syncRoot = new();

        public SingleThreadExecutor(ThreadPriority threadPriority = ThreadPriority.Normal)
        {
            _thread = new Thread(Work) { Priority = threadPriority, IsBackground = true };
            _thread.Start();
            
            var spinWait = new SpinWait();
            
            while (!_thread.IsAlive)
            {
                spinWait.SpinOnce();
            }
        }

        private void Work()
        {
            while (true)
            {
                try
                {
                    var task = _tasks.Take(_source.Token);
                    task.Run();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        public void Shutdown()
        {
            lock (_syncRoot)
            {
                if (_lock)
                {
                    return;
                }

                _lock = true;
                _source.Cancel(false);
                _thread.Join();
                
                Clean();
            }
        }

        private void Clean()
        {
            while (_tasks.TryTake(out var f))
            {
                f.Cancel();
            }
        }

        public T Execute<T>(T future) where T : IFuture
        {
            _tasks.Add(future);
            return future;
        }

        public void Join()
        {
            _thread.Join();
        }
    }
}
