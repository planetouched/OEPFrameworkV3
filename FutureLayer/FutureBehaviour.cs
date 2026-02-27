using System;
using FuturesV2.NotThreadSafe;
using OEPFrameworkV3.Utils;

namespace OEPFrameworkV3.FutureLayer
{
    public abstract class FutureBehaviour : Future
    {
        private readonly AttachWatcher _watcher = new();

        protected FutureBehaviour()
        {
            AddListenerOnFinalize(_ => { Drop(); });
        }

        protected void Attach(int loop, Action action)
        {
            _watcher.Attach(loop, action);
        }

        protected void Detach()
        {
            _watcher.DetachAll();
        }

        public override void ReUse()
        {
            base.ReUse();
            AddListenerOnFinalize(_ => { Drop(); });
        }

        private void Drop()
        {
            Detach();
        }
    }
}