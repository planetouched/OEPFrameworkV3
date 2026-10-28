using System;
using System.Collections.Generic;

namespace OEPFrameworkV3.Utils.DI
{
    public class DefaultContainer : ILocatorContainer
    {
        private readonly Dictionary<Type, object> _instances = new();
        private readonly Dictionary<Type, object> _factories = new();

        public T Register<T>() where T : class, new()
        {
            if (_factories.ContainsKey(typeof(T)))
            {
                throw new Exception("Type has already registered in factories");
            }

            var obj = new T();
            _instances.Add(typeof(T), obj);
            return obj;
        }

        public void Register<T>(Func<T> factory) where T : class
        {
            if (_instances.ContainsKey(typeof(T)))
            {
                throw new Exception("Type has already registered in instances");
            }

            _factories.Add(typeof(T), factory);
        }

        public void Register(object instance)
        {
            var type = instance.GetType();

            if (_factories.ContainsKey(type))
            {
                throw new Exception("Type has already registered in factories");
            }

            _instances.Add(type, instance);
        }

        public T Find<T>() where T : class
        {
            var type = typeof(T);

            if (_instances.TryGetValue(type, out var instance))
            {
                return (T)instance;
            }
            
            if (_factories.TryGetValue(type, out var factory))
            {
                return ((Func<T>)factory)();
            }

            return null;
        }

        public void Unregister<T>(bool destroy = true) where T : class
        {
            var type = typeof(T);
            if (_instances.ContainsKey(type))
            {
                if (destroy)
                {
                    if (_instances[type] is IDestroyable destroyable)
                    {
                        destroyable.Destroy();
                    }
                }

                _instances.Remove(type);
            }
            else if (_factories.ContainsKey(type))
            {
                _factories.Remove(type);
            }
        }

        public bool Contains<T>() where T : class
        {
            var type = typeof(T);
            return _factories.ContainsKey(type) || _instances.ContainsKey(type);
        }

        public void Destroy()
        {
            foreach (var instance in _instances.Values)
            {
                if (instance is IDestroyable destroyable)
                {
                    destroyable.Destroy();
                }
            }

            _instances.Clear();
            _factories.Clear();
        }
    }
}