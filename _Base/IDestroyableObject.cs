using System;
#if !DISABLE_FUTURES
using FuturesV2._Base;
#endif

namespace OEPFrameworkV3._Base
{
    public interface IDestroyableObject
    {
#if !DISABLE_FUTURES
        void Bind(IFuture future);
#endif
        void Bind(IDestroyableObject obj);
        bool IsAlive { get; }
        event Action<IDestroyableObject> Destroyed;
        void Destroy();
        void SetAlive();
    }
}
