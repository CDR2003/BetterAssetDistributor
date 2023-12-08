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
        
        public void Build( List<BadAssetGroup> groups, string outputPath )
        {
            this.RecalculateDependencies( groups );
            this.EnsureDirectory( outputPath );
            this.GenerateVersionId();
            this.BuildAssetBundles( groups, outputPath );
            this.GenerateAssetInfo( groups, outputPath );
            this.GenerateAssetState( groups, outputPath );
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

        private void GenerateAssetState( List<BadAssetGroup> groups, string outputPath )
        {
            var file = BadAssetStateFile.Create( groups );
            var filePath = Path.Join( outputPath, $"asset_state_{_versionId}.bad" );
            file.WriteToFile( filePath );
        }

        private void GenerateVersionId()
        {
            _versionId = DateTime.Now.ToString( "yyyyMMddHHmmss" );
        }

        private void GenerateAssetInfo( List<BadAssetGroup> groups, string outputPath )
        {
            var file = BadAssetInfoFile.Create( groups, outputPath );
            var filePath = Path.Join( outputPath, $"asset_info_{_versionId}.bad" );
            file.WriteToFile( filePath );
        }

        private void BuildAssetBundles( List<BadAssetGroup> groups, string outputPath )
        {
            var bundleBuilds = groups.ConvertAll( g => g.ToAssetBundleBuild() ).ToArray();
            var manifest = BuildPipeline.BuildAssetBundles( outputPath, bundleBuilds, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows64 );
            Assert.IsNotNull( manifest );
        }

        private void EnsureDirectory( string outputPath )
        {
            if( Directory.Exists( outputPath ) == false )
            {
                Directory.CreateDirectory( outputPath );
            }
        }
    }
}