using System;
using System.Collections.Generic;

namespace OEPFrameworkV3.Core
{
    public static class LoopManager
    {
        private static readonly List<Loop> _loops = new();
        public static int CurrentLoop { get; private set; }
        public static LoopTime CurrentLoopTime { get; set; } = new();
        public static uint CurrentFrame { get; private set; }

        public static void IncrementFrame()
        {
            CurrentFrame++;
        }

        public static int AddNewLoop()
        {
            _loops.Add(new Loop(_loops.Count));
            return _loops.Count - 1;
        }

        public static float GetTimeScale(int loop)
        {
            return _loops[loop].TimeScale;
        }

        public static void SetTimeScale(int loop, float timeScale)
        {
            if (timeScale < 0)
            {
                timeScale = 0;
            }

            _loops[loop].TimeScale = timeScale;
        }

        public static void SyncAction(int loop, Action action)
        {
            _loops[loop].SyncAction(action);
        }

        public static AttachInfo Attach(int loop, Action action, ITouchObject touchObject = null)
        {
            return _loops[loop].Attach(action, touchObject);
        }

        public static void Detach(AttachInfo attachInfo)
        {
            _loops[attachInfo.loopIdx].Detach(attachInfo);
        }

        public static void Clear(int loop)
        {
            _loops[loop].Clear();
        }

        public static void ClearAll()
        {
            foreach (var loop in _loops)
            {
                loop.Clear();
            }
        }

        public static void EnableLoop(int loop, bool enable)
        {
            _loops[loop].Enabled = enable;
        }

        public static void Call(int loop)
        {
            CurrentLoop = loop;
            var timeScale = _loops[loop].TimeScale;
            CurrentLoopTime.Setup(timeScale);

            _loops[loop].Call();
        }
    }
}