using System;
using OEPFrameworkV3.Timers._Base;

namespace OEPFrameworkV3.Timers
{
    public static class TimerUtil
    {
        public static ITimer Create(int loop, float period, Action<ITimer> method, object obj, bool once = false)
        {
            var timer = new DeltaTimeTimer(loop, period, method, once)
            {
                Obj = obj
            };
            
            return timer;
        }

        public static ITimer Create(int loop, float period, Action<ITimer> method, bool once = false)
        {
            return new DeltaTimeTimer(loop, period, method, once);
        }
    }
}