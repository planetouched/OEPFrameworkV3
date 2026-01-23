using System;
using FuturesV2.Utils.ThreadSafe;
using OEPFrameworkV3.Utils;

namespace OEPFrameworkV3._Base
{
    public interface IDestroyableObject
    {
        DestroyableWatcher GetDestroyableWatcher();
        FutureWatcher GetFutureWatcher();
        bool Alive { get; }
        event Action<IDestroyableObject> Destroyed;
        void Destroy();
        void SetAlive();
    }
}
