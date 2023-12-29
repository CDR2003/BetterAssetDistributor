using System.Collections.Generic;
using UnityEditor;

namespace RocketPunch.Bad
{
    public class BadAssetGroup
    {
        public readonly string name;

        public readonly BadAssetLocation location;
        
        public readonly List<BadAsset> assets;
        
        public BadAssetGroup( string name, BadAssetLocation location )
        {
            this.name = name;
            this.location = location;
            this.assets = new();
        }
        
#if UNITY_EDITOR
        public AssetBundleBuild ToAssetBundleBuild()
        {
            var assetNames = assets.ConvertAll( a => a.path ).ToArray();
            var addressableNames = assets.ConvertAll( a => a.guid ).ToArray();
            return new AssetBundleBuild
            {
                assetBundleName = name,
                assetNames = assetNames,
                addressableNames = addressableNames,
            };
        }
#endif

        public override string ToString()
        {
            return this.name;
        }
    }
}