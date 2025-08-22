using System;
using FuturesV2._Base;
using OEPFrameworkV3.Behaviours;

namespace OEPFrameworkV3.EssentialFutures
{
    public class UpdateLoopFuture : FutureBehaviour
    {
        private Action<UpdateLoopFuture> _updateAction;
        private int _loop;

        public UpdateLoopFuture()
        {
        }
        
        public UpdateLoopFuture(Action<UpdateLoopFuture> updateAction, int loop)
        {
            Initialize(updateAction, loop);
        }

        public IFuture Initialize(Action<UpdateLoopFuture> updateAction, int loop)
        {
            _loop = loop;
            _updateAction = updateAction;
            return this;
        }
        
        protected override void OnRun()
        {
            Attach(_loop, OnUpdate);
            Play();
        }

        private void OnUpdate()
        {
            _updateAction(this);
        }

        protected override void OnComplete()
        {
            _updateAction = null;
        }
    }
}
