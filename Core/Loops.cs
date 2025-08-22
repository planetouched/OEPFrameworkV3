namespace OEPFrameworkV3.Core
{
    public static class Loops
    {
        public static int Timer { get; private set; }
        public static int FixedUpdate { get; private set; }
        public static int Update { get; private set; }
        public static int LateUpdate { get; private set; }

        public static void Initialize()
        {
            Timer = LoopManager.AddNewLoop();
            FixedUpdate = LoopManager.AddNewLoop();
            Update = LoopManager.AddNewLoop();
            LateUpdate = LoopManager.AddNewLoop();
        }
    }
}
