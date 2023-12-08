using UnityEngine;

namespace RocketPunch.Bad
{
    public class BadBundleSyncLoadTask : BadSyncLoadTask
    {
        public readonly BadBundleInfo bundle;
        
        public BadBundleSyncLoadTask( BadBundleInfo bundle )
        {
            this.bundle = bundle;
        }
        
        public override void Run()
        {
            this.bundle.Load();
        }

        public override string ToString()
        {
            return $"Bundle: {this.bundle.name}";
        }
    }
}