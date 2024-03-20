using System.Collections.Generic;

namespace RocketPunch.Bad
{
    public class BadModificationChecker
    {
        public List<BadAsset> modifiedAssets { get; private set; }

        public List<BadAssetGroup> modifiedGroups { get; private set; }
        
        private readonly BadAssetStateFile _assetState;

        private readonly Dictionary<BadAsset, BadAssetGroup> _assetGroups;
        
        public BadModificationChecker( BadAssetStateFile assetState )
        {
            _assetState = assetState;
            _assetGroups = new Dictionary<BadAsset, BadAssetGroup>();

            this.modifiedAssets = new List<BadAsset>();
            this.modifiedGroups = new List<BadAssetGroup>();
        }
        
        public void CheckForModifiedAssets( List<BadAssetGroup> newGroups )
        {
            this.ResetAssetGroups( newGroups );
            this.modifiedAssets.Clear();
            this.modifiedGroups.Clear();
            foreach( var group in newGroups )
            {
                var modified = false;
                foreach( var newAsset in group.assets )
                {
                    var state = this.CheckAsset( newAsset );
                    if( state != BadAssetModificationState.Identical )
                    {
                        this.modifiedAssets.Add( newAsset );
                        modified = true;
                    }
                }

                if( modified )
                {
                    this.modifiedGroups.Add( group );
                }
            }
        }
        
        private BadAssetModificationState CheckAsset( BadAsset asset )
        {
            var group = _assetGroups.GetValueOrDefault( asset );
            return this.CheckAsset( asset, group, new HashSet<BadAsset>() );
        }
        
        private BadAssetModificationState CheckAsset( BadAsset asset, BadAssetGroup rootAssetGroup, HashSet<BadAsset> checkedAssets )
        {
            if( checkedAssets.Add( asset ) == false )
            {
                return BadAssetModificationState.Identical;
            }
            
            var chunk = _assetState.assets.GetValueOrDefault( asset.guid );
            if( chunk == null )
            {
                return BadAssetModificationState.BrandNew;
            }
            
            if( BadHashUtility.AreEqual( chunk.hash, asset.hash ) == false )
            {
                return BadAssetModificationState.Modified;
            }

            return this.CheckAssetDependencies( asset, rootAssetGroup, checkedAssets );
        }
        
        private BadAssetModificationState CheckAssetDependencies( BadAsset asset, BadAssetGroup rootAssetGroup, HashSet<BadAsset> checkedAssets )
        {
            foreach( var dependency in asset.dependencies )
            {
                var group = _assetGroups.GetValueOrDefault( dependency );
                if( group != null && group != rootAssetGroup )
                {
                    continue;
                }
                
                var state = this.CheckAsset( dependency, rootAssetGroup, checkedAssets );
                if( state != BadAssetModificationState.Identical )
                {
                    return BadAssetModificationState.Modified;
                }
            }
            
            return BadAssetModificationState.Identical;
        }

        private void ResetAssetGroups( List<BadAssetGroup> groups )
        {
            _assetGroups.Clear();
            foreach( var group in groups )
            {
                foreach( var asset in group.assets )
                {
                    _assetGroups.Add( asset, group );
                }
            }
        }
    }
}