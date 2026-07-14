using System.Collections.Generic;

namespace OEPFrameworkV3.MonoLayer
{
    public abstract class MonoWaitEx : MonoEx
    {
        #region Static
        
        private static readonly List<MonoWaitEx> _components = new();
        public static int AwaitingReadiness => _components.Count;

        public static void Process()
        {
            if (_components.Count == 0) return;

            for (int i = _components.Count - 1; i >= 0; i--)
            {
                if (!_components[i])
                {
                    _components.RemoveAt(i);
                }
                
                var c = _components[i];
                
                if (c.isActiveAndEnabled && c.InitializationCheck())
                {
                    _components.RemoveAt(i);
                    c.IsReady = true;
                    c.Initialize();
                }
            }
        }
        
        #endregion Static

        public override bool IsActive => isActiveAndEnabled && IsReady;
        public bool IsReady { get; private set; }

        private void Awake()
        {
            _components.Add(this);
            OnAwakeEx();
        }

        private void OnDestroy()
        {
            if (!IsReady)
            {
                _components.Remove(this);
            }
            
            OnDestroyEx();
        }

        protected abstract void OnAwakeEx();
        protected abstract bool InitializationCheck();
        protected abstract void Initialize();
        protected abstract void OnDestroyEx();
    }
}