using System.Collections.Generic;
using System.IO;

namespace RocketPunch.Bad
{
    public class BadUpdateBuilder : BadBuilderBase
    {
        private readonly BadAssetStateFile _assetState;
        
        public BadUpdateBuilder( string assetStatePath )
        {
            _assetState = BadAssetStateFile.ReadFromFile( assetStatePath );
        }

        public void Build( List<BadAssetGroup> groups )
        {
            this.GenerateVersionId();
            this.EnsureDirectory( BadSettings.instance.buildPath );
            
            var newAssetState = this.GenerateAssetState( groups );

            this.RecalculateDependencies( groups );
            
            var modifiedGroups = this.CalculateModifiedGroups( groups );
            var newBundleStates = this.BuildAssetBundles( modifiedGroups );
            newAssetState.CombineBundleStates( newBundleStates );
            this.GenerateAssetInfo( groups, _assetState.bundles, BadSettings.instance.buildPath );
            this.MoveRemoteBundles( modifiedGroups );
            this.MoveRemoteAssetInfo();
            this.DeleteManifests();
            this.WriteAssetState( newAssetState );
        }

        private List<BadAssetGroup> CalculateModifiedGroups( List<BadAssetGroup> groups )
        {
            var checker = new BadModificationChecker( _assetState );
            checker.CheckForModifiedAssets( groups );
            return checker.modifiedGroups;
        }
    }
}