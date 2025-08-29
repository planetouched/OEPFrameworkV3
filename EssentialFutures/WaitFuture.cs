using FuturesV2._Base;
using OEPFrameworkV3.Behaviours;
using OEPFrameworkV3.Core;

namespace OEPFrameworkV3.EssentialFutures
{
    public class WaitFuture : FutureBehaviour
    {
        private float _time;
        private int _loop;
        private float _eta;

        public WaitFuture()
        {
        }
        
        public WaitFuture(float time, int loop = -1)
        {
            Initialize(time, loop);
        }

        public IFuture Initialize(float time, int loop = -1)
        {
            _loop = loop == -1 ? Loops.Timer : loop;
            _time = time;
            return this;
        }

        protected override void OnRun()
        {
            _eta = 0;
            Attach(_loop, OnTick);
        }

        private void OnTick()
        {
            if (paused) return;
            
            _eta += LoopManager.CurrentLoopTime.deltaTime;
            
            if (_eta >= _time)
            {
                Complete();
            }
        }

        protected override void OnComplete()
        {
            Detach();
        }
    }
}
