using System;
using OEPFrameworkV3.Core;
using OEPFrameworkV3.Timers._Base;

namespace OEPFrameworkV3.Timers
{
    public class DeltaTimeTimer : TimerBase
    {
        private float _eta;
        private bool _skipOneFrame;

        public DeltaTimeTimer(int loop, float period, Action<ITimer> action, bool once) : base(period, action, once)
        {
            Attach(loop, OnUpdate);
            running = true;
        }

        private void OnUpdate()
        {
            if (!running)
            {
                _skipOneFrame = true;
                return;
            }
            
            _eta += LoopManager.CurrentLoopTime.DeltaTime;

            if (_eta >= period && _skipOneFrame)
            {
                _eta -= period;
                InnerCall();
            }

            _skipOneFrame = true;
        }

        public override void Reset()
        {
            _eta = 0;
        }
    }
}