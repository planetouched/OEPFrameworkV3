using FuturesV2._Base;
using FuturesV2.Enums;
using OEPFrameworkV3.Behaviours;
using OEPFrameworkV3.Core;

namespace OEPFrameworkV3.EssentialFutures
{
    public class DelayFuture : FutureBehaviour
    {
        private IFuture _future;
        
        private float _time;
        private int _loop;
        private float _eta;

        public DelayFuture()
        {
        }

        public DelayFuture(float time, IFuture future, int timerLoop = -1)
        {
            Initialize(time, future, timerLoop);
        }

        public IFuture Initialize(float time, IFuture future, int loop = -1)
        {
            _loop = loop == -1 ? Loops.Timer : loop;
            _time = time;
            _future = future;
            return this;
        }

        protected override void OnRun()
        {
            _future.AddListener(FutureCompletionState.Done, _ => { Complete(); });
            _eta = 0;
            Attach(_loop, OnTick);
        }

        private void OnTick()
        {
            if (paused) return;
            
            _eta += LoopManager.CurrentLoopTime.deltaTime;
            
            if (_eta >= _time)
            {
                Detach();
                _future.Run();
            }
        }

        protected override void OnComplete()
        {
            if (IsCancelled)
            {
                _future.Cancel();
            }
            
            Detach();
            _future = null;
        }
    }
}