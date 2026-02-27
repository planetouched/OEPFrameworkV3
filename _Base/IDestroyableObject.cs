using System;
using FuturesV2._Base;

namespace OEPFrameworkV3._Base
{
    public interface IDestroyableObject
    {
        void Bind(IFuture future);
        void Bind(IDestroyableObject obj);
        bool Alive { get; }
        event Action<IDestroyableObject> Destroyed;
        void Destroy();
        void SetAlive();
    }
}
