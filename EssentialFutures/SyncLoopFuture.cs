using FuturesV2._Base;
using FuturesV2.NotThreadSafe;
using OEPFrameworkV3.Core;

namespace OEPFrameworkV3.EssentialFutures
{
    public class SyncLoopFuture : Future
    {
        private int _loop;

        public SyncLoopFuture()
        {
        }
        
        public SyncLoopFuture(int loopType)
        {
            Initialize(loopType);
        }

        public IFuture Initialize(int loop)
        {
            _loop = loop;
            return this;
        }

        protected override void OnRun()
        {
            LoopManager.SyncAction(_loop, () => Complete());
        }

        protected override void OnComplete()
        {
        }
    }
}
