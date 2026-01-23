using System;
using OEPFrameworkV3._Base;
using OEPFrameworkV3.Utils;

namespace OEPFrameworkV3.Behaviours
{
    public abstract class LoopBehaviour : DestroyableObjectBase
    {
        private readonly AttachWatcher _watcher = new();

        protected void Attach(int loop, Action action)
        {
            if (!Alive)
            {
                throw new Exception("It is Dead");
            }
            
            _watcher.Attach(loop, action);
        }

        protected void Detach(int loop)
        {
            _watcher.Detach(loop);
        }

        protected void DetachAll()
        {
            _watcher.DetachAll();
        }
        
        protected override void OnDestroy()
        {
            DetachAll();
            base.OnDestroy();
        }
    }
}