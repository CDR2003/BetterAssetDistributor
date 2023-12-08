namespace RocketPunch.Bad
{
    public class BadLoadedAssetInfo
    {
        public readonly BadAssetInfo asset;
        
        public readonly UnityEngine.Object obj;

        public int referenceCount;
        
        public BadLoadedAssetInfo( BadAssetInfo asset, UnityEngine.Object obj )
        {
            this.asset = asset;
            this.obj = obj;
            this.referenceCount = 0;
        }

        public override string ToString()
        {
            return $"Asset '{this.asset.name}' ({this.asset.guid}) @ '{this.asset.bundle.name}': {this.referenceCount}";
        }
    }
}