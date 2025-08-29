using UnityEngine;

namespace OEPFrameworkV3.Core
{
    public class LoopTime
    {
        public float deltaTime;
        public float fixedDeltaTime;
        public float time;

        public virtual void Setup(float loopTimeScale)
        {
            deltaTime = Time.deltaTime * loopTimeScale;
            fixedDeltaTime = Time.fixedDeltaTime;
            time = Time.fixedDeltaTime;
        }

        public static void SetLoopTime<T>() where T : LoopTime, new()
        {
            LoopManager.CurrentLoopTime = new T();
        }
    }
}
