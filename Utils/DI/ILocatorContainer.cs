using System;

namespace OEPFrameworkV3.Utils.DI
{
    public interface ILocatorContainer
    {
        T Register<T>() where T : class, new();
        void Register<T>(Func<T> factory) where T : class;
        void Register(object instance);
        T Find<T>() where T : class;
        void Unregister<T>(bool destroy = true) where T : class;
        bool Contains<T>() where T : class;
        void Destroy();
    }
}