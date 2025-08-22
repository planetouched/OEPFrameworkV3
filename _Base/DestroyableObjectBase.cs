using System;
using FuturesV2.Utils.ThreadSafe;
#if REFVIEW
using RefViewer.Runtime;
#endif

namespace OEPFrameworkV3._Base
{
    public abstract class DestroyableObjectBase : 
#if REFVIEW
        ReferenceCounter,
#endif         
        IDestroyableObject
    {
        private DestroyableWatcher _timers;
        private FutureWatcher _futureWatcher;
        
        public DestroyableWatcher GetDestroyableWatcher()
        {
            return _timers ??= new DestroyableWatcher();
        }

        public FutureWatcher GetFutureWatcher()
        {
            return _futureWatcher ??= new FutureWatcher();
        }

        public bool Alive { get; private set; } = true;
        public event Action<IDestroyableObject> Destroyed;

        protected virtual void OnDestroy()
        {
        }
        
        public void Destroy()
        {
            if (!Alive)
            {
                return;
            }

            _timers?.DestroyAll();
            _futureWatcher?.CancelFutures();
            
            OnDestroy();
            Alive = false;
            Destroyed?.Invoke(this);
            Destroyed = null;
        }

        public virtual void SetAlive()
        {
            Alive = true;
        }
    }
}
