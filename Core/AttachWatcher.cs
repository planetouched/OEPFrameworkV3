using System;
using System.Collections.Generic;

namespace OEPFrameworkV3.Core
{
    public class AttachWatcher
    {
        private readonly List<AttachInfo> _attachInfos = new();

        public void Attach(int loop, Action action)
        {
            _attachInfos.Add(LoopManager.Attach(loop, action));
        }

        public void Detach(int loop)
        {
            for (int i = _attachInfos.Count - 1; i >= 0; i--)
            {
                if (_attachInfos[i].loopIdx == loop)
                {
                    LoopManager.Detach(_attachInfos[i]);
                    _attachInfos.RemoveAt(i);
                }
            }
        }        

        public void DetachAll()
        {
            foreach (var attachInfo in _attachInfos)
            {
                LoopManager.Detach(attachInfo);
            }
            
            _attachInfos.Clear();        
        }
    }
}