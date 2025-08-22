using OEPFrameworkV3._Base;

namespace OEPFrameworkV3.Timers._Base
{
    public interface ITimer : IDestroyableObject
    {
        object Obj { get; set; }
        void Reset();
        void Resume();
        void Pause();
        void Bind(IDestroyableObject binder);
    }
}