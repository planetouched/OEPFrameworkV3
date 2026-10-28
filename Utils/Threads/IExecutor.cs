using FuturesV2._Base;

namespace OEPFrameworkV3.Utils.Threads
{
    public interface IExecutor
    {
        int TaskCount { get; }
        void Shutdown();
        T Execute<T>(T future) where T : IFuture;
        void Join();
    }
}
