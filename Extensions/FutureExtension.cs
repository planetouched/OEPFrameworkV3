using FuturesV2._Base;
using FuturesV2.Utils.ThreadSafe;
using OEPFrameworkV3._Base;

namespace OEPFrameworkV3.Extensions
{
    public static class FutureExtension
    {
        public static IFuture Bind(this IFuture future, FutureWatcher futureWatcher)
        {
            futureWatcher.AddFuture(future);
            return future;
        }
        
        public static IFuture Bind(this IFuture future, IDestroyableObject puller)
        {
            puller.GetFutureWatcher().AddFuture(future);
            return future;
        }
    }
}