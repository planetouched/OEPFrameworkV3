using System.Collections.Generic;
using System.Threading;
using FuturesV2._Base;

namespace OEPFrameworkV3.Utils.Threads
{
    public class FixedThreadsExecutor : IExecutor
    {
        private readonly List<IExecutor> _threads = new();

        public int TaskCount
        {
            get
            {
                int sum = 0;

                foreach (var thread in _threads)
                {
                    sum += thread.TaskCount;
                }

                return sum;
            }
        }

        public FixedThreadsExecutor(int threadsCount, ThreadPriority threadPriority = ThreadPriority.Normal)
        {
            for (int i = 0; i < threadsCount; i++)
            {
                var tw = new SingleThreadExecutor(threadPriority);
                _threads.Add(tw);
            }
        }

        private IExecutor GetThread()
        {
            int count = _threads[0].TaskCount;

            if (_threads.Count == 1 || count == 0)
            {
                return _threads[0];
            }

            int idx = 0;
            for (int i = 1; i < _threads.Count; i++)
            {
                int test = _threads[i].TaskCount;

                if (test == 0)
                {
                    return _threads[i];
                }

                if (count > test)
                {
                    count = test;
                    idx = i;
                }
            }

            return _threads[idx];
        }

        public void Shutdown()
        {
            foreach (var thread in _threads)
            {
                thread.Shutdown();
            }
        }

        public T Execute<T>(T future) where T : IFuture
        {
            return GetThread().Execute(future);
        }

        public void Join()
        {
            foreach (var thread in _threads)
            {
                thread.Join();
            }
        }
    }
}
