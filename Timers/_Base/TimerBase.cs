﻿using System;
using OEPFrameworkV3._Base;
using OEPFrameworkV3.Behaviours;

namespace OEPFrameworkV3.Timers._Base
{
    public abstract class TimerBase : LoopBehaviour, ITimer
    {
        private Action<ITimer> _action;
        public object Obj { get; set; }

        protected readonly float period;
        private readonly bool _once;
        protected bool running;
        
        protected TimerBase(float period, Action<ITimer> action, bool once)
        {
            this.period = period;
            _action = action;
            _once = once;
        }
        
        protected void InnerCall()
        {
            _action?.Invoke(this);

            if (_once)
            {
                Destroy();
            }
        }
        
        public abstract void Reset();

        public void Resume()
        {
            running = true;
        }

        public void Pause()
        {
            running = false;
        }

        protected override void OnDestroy()
        {
            _action = null;
            base.OnDestroy();
        }
        
        public void Bind(IDestroyableObject binder)
        {
            binder.GetDestroyableWatcher().Add(this);
        }
    }
}