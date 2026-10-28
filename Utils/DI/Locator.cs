using System.Collections.Generic;

namespace OEPFrameworkV3.Utils.DI
{
    public static class Locator
    {
        private static readonly Dictionary<string, ILocatorContainer> _containers = new();

        public static void UnregisterAll(bool destroy = true)
        {
            if (destroy)
            {
                foreach (var container in _containers.Values)
                {
                    container.Destroy();
                }
            }
            
            _containers.Clear();
        }

        #region manage
        
        public static ILocatorContainer Register(string id, ILocatorContainer container)
        {
            _containers.Add(id, container);
            return container;
        }

        public static bool Contains(string id)
        {
            return _containers.ContainsKey(id);
        }

        public static ILocatorContainer Get(string id)
        {
            return _containers[id];
        }

        public static void Unregister(string id, bool destroy = true)
        {
            if (!Contains(id)) return;
            
            var c = _containers[id];

            if (destroy)
            {
                c.Destroy();
            }

            _containers.Remove(id);
        }
        
        #endregion

        public static void Resolve<T>(ref T pointer) where T : class
        {
            pointer = Resolve<T>();
        }
        
        public static T Resolve<T>() where T : class
        {
            foreach (var container in _containers.Values)
            {
                var value = container.Find<T>();
                if (value != null)
                {
                    return value;
                }
            }

            return null;            
        }
    }
}