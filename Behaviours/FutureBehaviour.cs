using System;
using FuturesV2.NotThreadSafe;
using OEPFrameworkV3.Core;

namespace OEPFrameworkV3.Behaviours
{
    public abstract class FutureBehaviour : Future
    {
        private readonly AttachWatcher _watcher = new();
        protected bool paused;

        protected FutureBehaviour()
        {
            AddListenerOnFinalize(_ => { Drop(); });
        }

        protected virtual void OnPause()
        {
        }

        protected virtual void OnPlay()
        {
        }

        protected void Attach(int loop, Action action)
        {
            _watcher.Attach(loop, action);
        }

        protected void Detach()
        {
            _watcher.DetachAll();
        }

        public void Pause()
        {
            if (!paused)
            {
                paused = true;
                OnPause();
            }
        }

        public void Play()
        {
            if (paused)
            {
                paused = false;
                OnPlay();
            }
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