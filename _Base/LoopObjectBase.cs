using System;
using System.Collections.Generic;
using FuturesV2._Base;
using FuturesV2.Utils.ThreadSafe;
using OEPFrameworkV3.Core;
using OEPFrameworkV3.Utils;
using RefViewer.Runtime;

namespace OEPFrameworkV3._Base
{
    public abstract class LoopObjectBase : 
#if REFVIEW
        ReferenceCounter,
#endif         
        IDestroyableObject
    {
        private DestroyableWatcher _destroyableWatcher;
        private FutureWatcher _futureWatcher;
        private List<AttachInfo> _attachInfos;

        public bool Alive { get; private set; } = true;
        public event Action<IDestroyableObject> Destroyed;

        public void Bind(IFuture future)
        {
            if (!Alive)
            {
                throw new Exception("Object is not alive");
            }
            
            _futureWatcher ??= new FutureWatcher();
            _futureWatcher.AddFuture(future);
        }

        public void Bind(IDestroyableObject obj)
        {
            if (!Alive)
            {
                throw new Exception("Object is not alive");
            }
            
            _destroyableWatcher ??= new DestroyableWatcher();
            _destroyableWatcher.Add(obj);
        }
        
        protected void Attach(int loop, Action action)
        {
            if (!Alive)
            {
                throw new Exception("Object is not alive");
            }
            
            _attachInfos ??= new();
            _attachInfos.Add(LoopManager.Attach(loop, action));
        }

        protected void Detach(int loop)
        {
            if (!Alive)
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
            if (!Alive)
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
            if (!Alive)
            {
                return;
            }

            DetachAll();
            _destroyableWatcher?.DestroyAll();
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
