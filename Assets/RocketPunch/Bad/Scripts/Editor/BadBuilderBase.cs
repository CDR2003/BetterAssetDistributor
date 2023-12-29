using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEngine.Assertions;

namespace RocketPunch.Bad
{
    public class BadBuilderBase
    {
        protected string _versionId;
        
        protected void GenerateVersionId()
        {
            _versionId = DateTime.Now.ToString( "yyyyMMddHHmmss" );
        }
        
        protected void EnsureDirectory( string outputPath )
        {
            if( Directory.Exists( outputPath ) == false )
            {
                Directory.CreateDirectory( outputPath );
            }
        }
        
        protected BadAssetStateFile GenerateAssetState( List<BadAssetGroup> groups )
        {
            var file = BadAssetStateFile.Create( groups );
            var filename = BadAssetStateFile.GetFilename( _versionId );
            var filePath = Path.Join( BadSettings.instance.buildPath, filename );
            return file;
        }
        
        protected void WriteAssetState( BadAssetStateFile file )
        {
            var filename = BadAssetStateFile.GetFilename( _versionId );
            var filePath = Path.Join( BadSettings.instance.buildPath, filename );
            file.WriteToFile( filePath );
        }
        
        protected BadAssetInfoFile GenerateAssetInfo( List<BadAssetGroup> groups, Dictionary<string, BadBundleStateChunk> bundleStates, string path )
        {
            var file = BadAssetInfoFile.Create( groups, bundleStates, path );
            var filename = BadAssetInfoFile.GetFilename( _versionId );
            var filePath = Path.Join( path, filename );
            file.WriteToFile( filePath );

            return file;
        }
        
        protected Dictionary<string, BadBundleStateChunk> BuildAssetBundles( List<BadAssetGroup> groups )
        {
            var buildPath = BadSettings.instance.buildPath;
            var bundleBuilds = groups.ConvertAll( g => g.ToAssetBundleBuild() ).ToArray();
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var manifest = CompatibilityBuildPipeline.BuildAssetBundles( buildPath, bundleBuilds, BuildAssetBundleOptions.DeterministicAssetBundle, buildTarget );
            Assert.IsNotNull( manifest );

            return this.CalculateBundleStates( groups );
        }
        
        protected void DeleteManifests()
        {
            var files = Directory.GetFiles( BadSettings.instance.buildPath );
            foreach( var file in files )
            {
                if( file.EndsWith( ".bad" ) == false )
                {
                    File.Delete( file );
                }
            }
        }

        protected void GenerateVersionInfo( string outputPath )
        {
            var newVersion = BadSettings.instance.version.GetNextVersion();
            BadSettings.instance.version = newVersion;
            
            var versionInfo = new BadVersionInfo();
            versionInfo.version = newVersion.ToString();
            versionInfo.assetInfoFilePath = BadAssetInfoFile.GetFilename( _versionId );
            versionInfo.WriteToFile( Path.Join( outputPath, BadVersionInfo.Filename ) );
        }

        protected void MoveRemoteBundles( List<BadAssetGroup> groups )
        {
            var remoteGroups = groups.FindAll( g => g.location == BadAssetLocation.Remote );
            foreach( var group in remoteGroups )
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

        protected void MoveRemoteAssetInfo()
        {
            var filename = BadAssetInfoFile.GetFilename( _versionId );
            var sourcePath = BadPathHelper.GetBuildPath( filename );
            var destinationPath = BadPathHelper.GetRemoteBuildPath( filename );
            if( File.Exists( destinationPath ) )
            {
                File.Delete( destinationPath );
            }
            File.Move( sourcePath, destinationPath );
        }

        protected void RecalculateDependencies( List<BadAssetGroup> groups )
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
        
        private Dictionary<string, BadBundleStateChunk> CalculateBundleStates( List<BadAssetGroup> groups )
        {
            var bundleStates = new Dictionary<string, BadBundleStateChunk>();
            foreach( var group in groups )
            {
                var bundleState = new BadBundleStateChunk();
                bundleState.name = group.name;
                
                var path = BadPathHelper.GetBuildPath( group.name );
                bundleState.hash = BadHashUtility.ComputeXXHash( path, out bundleState.size );
                
                bundleStates.Add( bundleState.name, bundleState );
            }
            return bundleStates;
        }
    }
}