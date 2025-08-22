namespace OEPFrameworkV3.Core
{
    public static class LTime
    {
        public static float deltaTime => LoopManager.CurrentLoopTime.deltaTime;
        public static float fixedDeltaTime => LoopManager.CurrentLoopTime.fixedDeltaTime;
    }
}