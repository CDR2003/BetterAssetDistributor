using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RocketPunch.Bad
{
    public class BadIncrementalUpdateBuilder : BadBuilderBase
    {
        private readonly BadAssetStateFile _assetState;
        
        public BadIncrementalUpdateBuilder( string assetStatePath )
        {
            _assetState = BadAssetStateFile.ReadFromFile( assetStatePath );
        }

        public void Build( List<BadAssetGroup> groups )
        {
            this.GenerateVersionId();
            this.EnsureDirectory( BadSettings.instance.buildPath );
            
            var newAssetState = this.GenerateAssetState( groups );
            newAssetState.bundles = _assetState.bundles;

            var modifiedGroups = this.CalculateModifiedGroups( groups );
            var buildGroups = this.GetAllDependencyGroups( modifiedGroups, groups );
            this.RecalculateDependencies( groups );
            
            var newBundleStates = this.BuildAssetBundles( buildGroups );
            this.ClearUnnecessaryBundleStates( newBundleStates, modifiedGroups );
            
            newAssetState.CombineBundleStates( newBundleStates );
            
            this.GenerateAssetInfo( groups, _assetState.bundles, BadSettings.instance.buildPath );

            this.MoveBundles( modifiedGroups );
            this.MoveRemoteAssetInfo();
            this.DeleteManifests();
            this.WriteAssetState( newAssetState, false );
        }

        private List<BadAssetGroup> CalculateModifiedGroups( List<BadAssetGroup> groups )
        {
            var checker = new BadModificationChecker( _assetState );
            checker.CheckForModifiedAssets( groups );
            return checker.modifiedGroups;
        }
        
        private List<BadAssetGroup> GetAllDependencyGroups( List<BadAssetGroup> groups, List<BadAssetGroup> allGroups )
        {
            var assetGroups = new Dictionary<BadAsset, BadAssetGroup>();
            foreach( var group in allGroups )
            {
                foreach( var asset in group.assets )
                {
                    assetGroups[asset] = group;
                }
            }
            
            var dependencyGroups = new HashSet<BadAssetGroup>();
            foreach( var group in groups )
            {
                foreach( var asset in group.assets )
                {
                    this.FillInDependencies( group, asset, assetGroups, dependencyGroups );
                }
            }
            
            return dependencyGroups.ToList();
        }

        private void FillInDependencies( BadAssetGroup group, BadAsset asset,
            Dictionary<BadAsset, BadAssetGroup> assetGroups, HashSet<BadAssetGroup> allDependencies )
        {
            allDependencies.Add( group );
            foreach( var dependency in asset.dependencies )
            {
                var dependencyGroup = assetGroups.GetValueOrDefault( dependency );
                if( dependencyGroup != null )
                {
                    this.FillInDependencies( dependencyGroup, dependency, assetGroups, allDependencies );
                }
                else
                {
                    this.FillInDependencies( group, dependency, assetGroups, allDependencies );
                }
            }
        }

        private void MoveBundles( List<BadAssetGroup> groups )
        {
            foreach( var group in groups )
            {
                var sourcePath = BadPathHelper.GetBuildPath( group.name );
                var destinationPath = BadPathHelper.GetRemoteBuildPath( group.name );
                if( File.Exists( destinationPath ) )
                {
                    File.Delete( destinationPath );
                }
                File.Move( sourcePath, destinationPath );
            }
            
            this.GenerateVersionInfo( BadSettings.instance.remoteBuildPath );
        }

        private void ClearUnnecessaryBundleStates( Dictionary<string, BadBundleStateChunk> bundleStates, List<BadAssetGroup> downloadGroups )
        {
            var unnecessaryBundleNames = new HashSet<string>( bundleStates.Count );
            foreach( var pair in bundleStates )
            {
                var bundleName = pair.Key;
                if( downloadGroups.Find( g => g.name == bundleName ) == null )
                {
                    unnecessaryBundleNames.Add( bundleName );
                }
            }
            
            foreach( var bundleName in unnecessaryBundleNames )
            {
                bundleStates.Remove( bundleName );
            }
        }
    }
}