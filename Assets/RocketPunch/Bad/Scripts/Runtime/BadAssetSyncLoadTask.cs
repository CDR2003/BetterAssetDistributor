using System;

namespace RocketPunch.Bad
{
    public class BadAssetSyncLoadTask : BadSyncLoadTask
    {
        public readonly BadAssetInfo asset;

        public BadAssetSyncLoadTask( BadAssetInfo asset )
        {
            this.asset = asset;
        }
        
        public override void Run()
        {
            this.asset.Load();
        }

        public override string ToString()
        {
            return $"Asset: {this.asset.name} ({this.asset.guid}) @ '{this.asset.bundle.name}'";
        }
    }
}