using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace RocketPunch.Bad
{
    public class BadBuilder
    {
        private string _versionId;
        
        public BadBuilder()
        {
        }
        
        public void Build( List<BadAssetGroup> groups )
        {
            this.RecalculateDependencies( groups );
            this.EnsureDirectory( BadSettings.instance.buildPath );
            this.GenerateVersionId();
            this.GenerateAssetState( groups );
            this.BuildAssetBundles( groups );
            this.MoveLocalBundles( groups );
            this.MoveRemoteBundles( groups );
        }

        private void RecalculateDependencies( List<BadAssetGroup> groups )
        {
            var bundledAssetGuids = new HashSet<BadAsset>( groups.SelectMany( g => g.assets ) );
            foreach( var group in groups )
            {
                foreach( var asset in group.assets )
                {
                    this.RecalculateDependencies( asset, bundledAssetGuids );
                }
            }
        }

        private void RecalculateDependencies( BadAsset asset, HashSet<BadAsset> bundledAssetGuids )
        {
            var dependencies = this.GetAllDependencies( asset );
            dependencies.RemoveAll( d => bundledAssetGuids.Contains( d ) == false );
            asset.dependencies = dependencies;
        }

        private List<BadAsset> GetAllDependencies( BadAsset asset )
        {
            var allDependencies = new List<BadAsset>();
            this.CollectAllDependenciesRecursive( asset, allDependencies );
            return allDependencies;
        }
        
        private void CollectAllDependenciesRecursive( BadAsset asset, List<BadAsset> dependencies )
        {
            foreach( var dependency in asset.dependencies )
            {
                if( dependencies.Contains( dependency ) == false )
                {
                    this.CollectAllDependenciesRecursive( dependency, dependencies );
                    dependencies.Add( dependency );
                }
            }
        }

        private void GenerateAssetState( List<BadAssetGroup> groups )
        {
            var file = BadAssetStateFile.Create( groups );
            var filePath = Path.Join( BadSettings.instance.buildPath, $"asset_state_{_versionId}.bad" );
            file.WriteToFile( filePath );
        }

        private void GenerateVersionId()
        {
            _versionId = DateTime.Now.ToString( "yyyyMMddHHmmss" );
        }

        private void BuildAssetBundles( List<BadAssetGroup> groups )
        {
            var buildPath = BadSettings.instance.buildPath;
            var bundleBuilds = groups.ConvertAll( g => g.ToAssetBundleBuild() ).ToArray();
            var manifest = BuildPipeline.BuildAssetBundles( buildPath, bundleBuilds, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows64 );
            Assert.IsNotNull( manifest );
        }

        private void MoveLocalBundles( List<BadAssetGroup> groups )
        {
            var localGroups = groups.FindAll( g => g.location == BadAssetLocation.Local );
            foreach( var group in localGroups )
            {
                var sourcePath = BadPathHelper.GetBuildPath( group.name );
                var destinationPath = BadPathHelper.GetLocalAssetPath( group.name );
                File.Move( sourcePath, destinationPath );
            }
            
            var path = Path.Join( Application.streamingAssetsPath, BadSettings.instance.localAssetPath );
            this.GenerateAssetInfo( localGroups, path );
            this.GenerateVersionInfo( localGroups, path );
        }

        private void MoveRemoteBundles( List<BadAssetGroup> groups )
        {
            var remoteGroups = groups.FindAll( g => g.location == BadAssetLocation.Remote );
            foreach( var group in remoteGroups )
            {
                var sourcePath = BadPathHelper.GetBuildPath( group.name );
                var destinationPath = BadPathHelper.GetRemoteBuildPath( group.name );
                File.Move( sourcePath, destinationPath );
            }
            
            this.GenerateAssetInfo( remoteGroups, BadSettings.instance.remoteBuildPath );
            this.GenerateVersionInfo( remoteGroups, BadSettings.instance.remoteBuildPath );
        }

        private void GenerateVersionInfo( List<BadAssetGroup> groups, string outputPath )
        {
            var newVersion = BadSettings.instance.version.GetNextVersion();
            BadSettings.instance.version = newVersion;
            
            var versionInfo = new BadVersionInfo();
            versionInfo.version = newVersion.ToString();
            versionInfo.assetInfoFilePath = $"asset_info_{_versionId}.bad";
            versionInfo.WriteToFile( Path.Join( outputPath, BadVersionInfo.Filename ) );
        }

        private void EnsureDirectory( string outputPath )
        {
            if( Directory.Exists( outputPath ) == false )
            {
                Directory.CreateDirectory( outputPath );
            }
        }
        
        private void GenerateAssetInfo( List<BadAssetGroup> groups, string outputPath )
        {
            var file = BadAssetInfoFile.Create( groups, outputPath );
            var filePath = Path.Join( outputPath, $"asset_info_{_versionId}.bad" );
            file.WriteToFile( filePath );
        }
    }
}