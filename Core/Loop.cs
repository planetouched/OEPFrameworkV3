using System;
using System.Collections.Generic;

namespace OEPFrameworkV3.Core
{
    public class Loop
    {
        private struct AttachRequest
        {
            public readonly Action action;
            public readonly int attachIdx;
            public readonly ITouchObject touchObject;

            public AttachRequest(Action action, int attachIdx, ITouchObject touchObject)
            {
                this.action = action;
                this.attachIdx = attachIdx;
                this.touchObject = touchObject;
            }
        }
        
        private struct LoopInfo
        {
            public readonly Action action;
            public readonly int attachIdx;
            public readonly ITouchObject touchObject;

            public LoopInfo(AttachRequest ar)
            {
                action = ar.action;
                attachIdx = ar.attachIdx;
                touchObject = ar.touchObject;
            }
        }

        private readonly List<Action> _syncActions = new();
        private readonly List<LoopInfo> _loops = new(1024);
        private readonly List<AttachRequest> _attachRequests = new(64);
        private readonly List<AttachInfo> _detachRequests = new(64);

        private int _attachIdx;
        private bool _clearFlag;
        private bool _syncActionAdded;

        private readonly object _syncRoot = new();
        public float TimeScale { get; set; } = 1;
        public bool Enabled { get; set; } = true;

        private readonly int _loopIdx;
        
        public Loop(int loopIdx)
        {
            _loopIdx = loopIdx;
        }

        public void SyncAction(Action action)
        {
            lock (_syncRoot)
            {
                _syncActions.Add(action);
            }

            _syncActionAdded = true;
        }

        public AttachInfo Attach(Action action, ITouchObject touchObject)
        {
            var ar = new AttachRequest(action, _attachIdx, touchObject);

            var ai = new AttachInfo
            {
                loopIdx = _loopIdx,
                attachIdx = _attachIdx,
                action = action
            };

            _attachRequests.Add(ar);
            _attachIdx++;
            return ai;
        }

        public void Detach(AttachInfo attachInfo)
        {
            if (attachInfo.loopIdx != _loopIdx)
            {
                throw new Exception("Loop mismatch");
            }

            _detachRequests.Add(attachInfo);
        }

        private void InnerCall(List<LoopInfo> actions)
        {
            foreach (var loopInfo in actions)
            {
                bool call = true;

                //если отписываемся в этом же цикле (редкая ситуация)
                if (_detachRequests.Count > 0)
                {
                    foreach (var ai in _detachRequests)
                    {
                        if (ai.action == loopInfo.action)
                        {
                            call = false;
                            break;
                        }
                    }
                }

                if (!call) continue;
                
                if (loopInfo.touchObject != null)
                {
                    if (!loopInfo.touchObject.IsAlive)
                    {
                        var detach = new AttachInfo
                        {
                            loopIdx = _loopIdx,
                            action = loopInfo.action,
                            attachIdx = loopInfo.attachIdx
                        };
                            
                        Detach(detach);
                        continue;
                    }

                    if (loopInfo.touchObject.IsActive)
                    {
                        loopInfo.action.Invoke();
                    }
                }
                else
                {
                    loopInfo.action.Invoke();
                }

                if (_clearFlag)
                {
                    break;
                }
            }
        }

        public void Clear()
        {
            _clearFlag = true;
            InnerClear();
        }

        private void InnerClear()
        {
            _loops.Clear();
            _attachRequests.Clear();
            _detachRequests.Clear();

            lock (_syncRoot)
            {
                _syncActions.Clear();
            }
        }

        private int BinarySearch(List<LoopInfo> loops, int findValue)
        {
            int left = 0;
            int right = loops.Count - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                int current = loops[mid].attachIdx;

                if (current == findValue)
                {
                    return mid;
                }

                if (current < findValue)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return -1;
        } 

        private bool ModifyLoops(out List<LoopInfo> newLoops)
        {
            newLoops = null;

            if (_attachRequests.Count > 0)
            {
                newLoops = new List<LoopInfo>();
                foreach (var ar in _attachRequests)
                {
                    newLoops.Add(new LoopInfo(ar));
                }

                _attachRequests.Clear();
            }

            if (_detachRequests.Count > 0)
            {
                foreach (var detachInfo in _detachRequests)
                {
                    if (newLoops != null)
                    {
                        var newRealIdx = BinarySearch(newLoops, detachInfo.attachIdx);
                        if (newRealIdx >= 0)
                        {
                            newLoops.RemoveAt(newRealIdx);
                            continue;
                        }
                    }

                    var realIdx = BinarySearch(_loops, detachInfo.attachIdx);
                    if (realIdx >= 0)
                    {
                        _loops.RemoveAt(realIdx);
                    }
                }

                _detachRequests.Clear();
            }

            return newLoops != null;
        }

        private void ProcessSync()
        {
            if (!_syncActionAdded)
            {
                return;
            }

            _syncActionAdded = false;

            List<Action> actions;
            lock (_syncRoot)
            {
                actions = new(_syncActions);
                _syncActions.Clear();
            }

            foreach (var action in actions)
            {
                if (_clearFlag)
                {
                    break;
                }

                action();
            }
        }

        public void Call()
        {
            if (!Enabled) return;
            
            ProcessSync();

            //что-то могло быть добавлено в других циклах
            if (ModifyLoops(out var newLoops))
            {
                _loops.AddRange(newLoops);
            }

            _clearFlag = false;
            
            InnerCall(_loops);

            //что-то добавилось в этом цикле?
            while (_attachRequests.Count > 0)
            {
                if (ModifyLoops(out var newLoopActions))
                {
                    _loops.AddRange(newLoopActions);
                    //вызываем только новые
                    InnerCall(newLoopActions);
                }
            }
            
            _clearFlag = false;
        }
    }
}