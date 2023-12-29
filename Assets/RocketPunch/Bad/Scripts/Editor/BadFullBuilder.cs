using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Assertions;

namespace RocketPunch.Bad
{
    public class BadFullBuilder : BadBuilderBase
    {
        private BadAssetStateFile _assetState;
        
        public BadFullBuilder()
        {
        }
        
        public void Build( List<BadAssetGroup> groups )
        {
            this.GenerateVersionId();
            this.EnsureDirectory( BadSettings.instance.buildPath );
            
            _assetState = this.GenerateAssetState( groups );
            
            this.RecalculateDependencies( groups );
            
            _assetState.bundles = this.BuildAssetBundles( groups );
            this.GenerateAssetInfo( groups, _assetState.bundles, BadSettings.instance.buildPath );
            this.MoveLocalBundles( groups );
            this.MoveRemoteBundles( groups );
            this.MoveRemoteAssetInfo();
            this.DeleteManifests();
            this.WriteAssetState( _assetState, true );
        }

        private void MoveLocalBundles( List<BadAssetGroup> groups )
        {
            var localGroups = groups.FindAll( g => g.location == BadAssetLocation.Local );
            foreach( var group in localGroups )
            {
                var sourcePath = BadPathHelper.GetBuildPath( group.name );
                var destinationPath = BadPathHelper.GetLocalAssetPath( group.name );
                if( File.Exists( destinationPath ) )
                {
                    File.Delete( destinationPath );
                }
                File.Move( sourcePath, destinationPath );
            }
            
            var path = Path.Join( Application.streamingAssetsPath, BadSettings.instance.localAssetPath );
            this.GenerateAssetInfo( localGroups, _assetState.bundles, path );
            this.GenerateVersionInfo( path );
        }
    }
}