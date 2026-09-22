using OEPFrameworkV3.Core;
using UnityEngine;

namespace OEPFrameworkV3.MonoLayer
{
    public class MonoTouchObject : ITouchObject
    {
        public bool IsActive => _monoBehaviour.isActiveAndEnabled;
        public bool IsAlive => _monoBehaviour;

        private readonly MonoBehaviour _monoBehaviour;

        public MonoTouchObject(MonoBehaviour monoBehaviour)
        {
            _monoBehaviour = monoBehaviour;
        }
    }
}