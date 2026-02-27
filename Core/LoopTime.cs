namespace OEPFrameworkV3.Core
{
    public class LoopTime
    {
        public float DeltaTime { get; private set; }
        public float FixedDeltaTime { get; private set;}
        public float Time { get; private set;}

        public virtual void Setup(float loopTimeScale)
        {
            DeltaTime = UnityEngine.Time.deltaTime * loopTimeScale;
            FixedDeltaTime = UnityEngine.Time.fixedDeltaTime;
            Time = UnityEngine.Time.fixedDeltaTime;
        }

        public static void SetLoopTime<T>() where T : LoopTime, new()
        {
            LoopManager.CurrentLoopTime = new T();
        }
    }
}
