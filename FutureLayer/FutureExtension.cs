using FuturesV2._Base;
using OEPFrameworkV3._Base;

namespace OEPFrameworkV3.FutureLayer
{
    public static class FutureExtension
    {
        public static IFuture BindTo(this IFuture future, IDestroyableObject obj)
        {
            obj.Bind(future);
            return future;
        }
    }
}