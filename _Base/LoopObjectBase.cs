using System;
using System.Collections.Generic;
#if !DISABLE_FUTURES
using FuturesV2._Base;
using FuturesV2.Utils.ThreadSafe;
#endif
using OEPFrameworkV3.Core;
using OEPFrameworkV3.Utils;
#if REFVIEW
using RefViewer.Runtime;
#endif

namespace OEPFrameworkV3._Base
{
    public abstract class LoopObjectBase : 
#if REFVIEW
        ReferenceCounter,
#endif         
        IDestroyableObject
    {
        private DestroyableWatcher _destroyableWatcher;
#if !DISABLE_FUTURES
        private FutureWatcher _futureWatcher;
#endif
        private List<AttachInfo> _attachInfos;

        public bool IsAlive { get; private set; } = true;
        public event Action<IDestroyableObject> Destroyed;

#if !DISABLE_FUTURES        
        public void Bind(IFuture future)
        {
            if (!IsAlive)
            {
                throw new Exception("Object is not alive");
            }
            
            _futureWatcher ??= new FutureWatcher();
            _futureWatcher.AddFuture(future);
        }
#endif        

        public void Bind(IDestroyableObject obj)
        {
            if (!IsAlive)
            {
                throw new Exception("Object is not alive");
            }
            
            _destroyableWatcher ??= new DestroyableWatcher();
            _destroyableWatcher.Add(obj);
        }
        
        protected void Attach(int loop, Action action)
        {
            if (!IsAlive)
            {
                throw new Exception("Object is not alive");
            }
            
            _attachInfos ??= new();
            _attachInfos.Add(LoopManager.Attach(loop, action));
        }

        protected void Detach(int loop)
        {
            if (!IsAlive)
            {
                throw new Exception("Object is not alive");
            }
            
            if (_attachInfos == null) return;
            
            for (int i = _attachInfos.Count - 1; i >= 0; i--)
            {
                if (_attachInfos[i].loopIdx == loop)
                {
                    LoopManager.Detach(_attachInfos[i]);
                    _attachInfos.RemoveAt(i);
                }
            }
        }        

        protected void DetachAll()
        {
            if (!IsAlive)
            {
                throw new Exception("Object is not alive");
            }
            
            if (_attachInfos == null) return;
            
            foreach (var attachInfo in _attachInfos)
            {
                LoopManager.Detach(attachInfo);
            }
            
            _attachInfos.Clear();        
        }        
        
        protected virtual void OnDestroy()
        {
        }
        
        public void Destroy()
        {
            if (!IsAlive)
            {
                return;
            }

            DetachAll();
            _destroyableWatcher?.DestroyAll();
            
#if !DISABLE_FUTURES
            _futureWatcher?.CancelFutures();
#endif
            
            OnDestroy();
            IsAlive = false;
            Destroyed?.Invoke(this);
            Destroyed = null;
        }

        public virtual void SetAlive()
        {
            IsAlive = true;
        }
    }
}
