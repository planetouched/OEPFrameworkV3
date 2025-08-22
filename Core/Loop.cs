using System;
using System.Collections.Generic;

namespace OEPFrameworkV3.Core
{
    public class Loop
    {
        private struct AttachRequest
        {
            public Action action;
            public int attachIdx;
        }

        private readonly List<Action> _syncActions = new();
        private readonly List<Action> _loopHandlers = new(1024);
        private readonly List<int> _attachIndexes = new(1024);

        private readonly List<AttachRequest> _attachRequests = new(64);
        private readonly List<AttachInfo> _detachRequests = new(64);

        private int _attachIdx;
        private bool _clearFlag;
        private bool _syncActionAdded;

        private readonly object _syncRoot = new();
        public float TimeScale { get; set; } = 1;

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

        public AttachInfo Attach(Action action)
        {
            var ar = new AttachRequest
            {
                action = action,
                attachIdx = _attachIdx
            };

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

        private void InnerCall(List<Action> actions)
        {
            foreach (var action in actions)
            {
                bool call = true;

                //если отписываемся в этом же цикле (редкая ситуация)
                if (_detachRequests.Count > 0)
                {
                    foreach (var ai in _detachRequests)
                    {
                        if (ai.action == action)
                        {
                            call = false;
                            break;
                        }
                    }
                }

                if (call)
                {
                    action.Invoke();

                    if (_clearFlag)
                    {
                        break;
                    }
                }
            }

            if (_clearFlag)
            {
                _clearFlag = false;
                InnerClear();
            }
        }

        public void Clear()
        {
            _clearFlag = true;
        }

        private void InnerClear()
        {
            _loopHandlers.Clear();
            _attachIndexes.Clear();
            _attachRequests.Clear();
            _detachRequests.Clear();

            lock (_syncRoot)
            {
                _syncActions.Clear();
            }
        }

        private bool ModifyLoops(out List<Action> newActions, out List<int> newAttachIndexes)
        {
            newActions = null;
            newAttachIndexes = null;

            if (_attachRequests.Count > 0)
            {
                newActions = new List<Action>();
                newAttachIndexes = new List<int>();

                foreach (var ar in _attachRequests)
                {
                    newActions.Add(ar.action);
                    newAttachIndexes.Add(ar.attachIdx);
                }

                _attachRequests.Clear();
            }

            if (_detachRequests.Count > 0)
            {
                foreach (var attachInfo in _detachRequests)
                {
                    if (newActions != null)
                    {
                        var newRealIdx = newAttachIndexes.BinarySearch(attachInfo.attachIdx);
                        if (newRealIdx >= 0)
                        {
                            newActions.RemoveAt(newRealIdx);
                            newAttachIndexes.RemoveAt(newRealIdx);
                            continue;
                        }
                    }

                    var realIdx = _attachIndexes.BinarySearch(attachInfo.attachIdx);
                    if (realIdx >= 0)
                    {
                        _loopHandlers.RemoveAt(realIdx);
                        _attachIndexes.RemoveAt(realIdx);
                    }
                }

                _detachRequests.Clear();
            }

            return newActions != null;
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
                    return;
                }

                action();
            }
        }

        public void Call()
        {
            if (_clearFlag)
            {
                _clearFlag = false;
                InnerClear();
            }

            ProcessSync();

            //что-то могло быть добавлено в других циклах
            if (ModifyLoops(out var newActions, out var newAttachIndexes))
            {
                _loopHandlers.AddRange(newActions);
                _attachIndexes.AddRange(newAttachIndexes);
            }

            InnerCall(_loopHandlers);

            //что-то добавилось в этом цикле?
            while (_attachRequests.Count > 0)
            {
                if (ModifyLoops(out var newLoopActions, out var newLoopAttachIndexes))
                {
                    _loopHandlers.AddRange(newLoopActions);
                    _attachIndexes.AddRange(newLoopAttachIndexes);
                    //вызываем только новые
                    InnerCall(newLoopActions);
                }
            }
        }
    }
}