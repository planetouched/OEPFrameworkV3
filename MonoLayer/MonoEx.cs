using System;
using System.Collections.Generic;
using OEPFrameworkV3.Core;
using UnityEngine;

namespace OEPFrameworkV3.MonoLayer
{
    public abstract class MonoEx : MonoBehaviour, ITouchObject
    {
        private List<AttachInfo> _attachInfos;

        public bool IsActive { get; private set; }
        public bool IsAlive => this;

        protected void Attach(int loop, Action action)
        {
            _attachInfos ??= new();
            _attachInfos.Add(LoopManager.Attach(loop, action, this));
        }

        protected abstract void OnEnableEx();
        protected abstract void OnDisableEx();
        
        private void OnEnable()
        {
            IsActive = true;
            OnEnableEx();
        }

        private void OnDisable()
        {
            IsActive = false;
            OnDisableEx();
        }

        protected void Detach(int loop)
        {
            if (_attachInfos == null) return;
            
            for (int i = _attachInfos.Count - 1; i >= 0; i--)
            {
                if (_attachInfos[i].loopIdx == loop)
                {
                    LoopManager.Detach(_attachInfos[i]);
                    _attachInfos.RemoveAt(i);
                }
            }
        }        

        protected void DetachAll()
        {
            if (_attachInfos == null) return;
            
            foreach (var attachInfo in _attachInfos)
            {
                LoopManager.Detach(attachInfo);
            }
            
            _attachInfos.Clear();        
        }
    }
}