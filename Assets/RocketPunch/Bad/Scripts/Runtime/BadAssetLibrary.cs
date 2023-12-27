using System;
using System.Collections.Generic;

namespace RocketPunch.Bad
{
    public class BadAssetLibrary
    {
        public static BadAssetLibrary instance { get; private set; }
        
        private Dictionary<string, BadBundleInfo> _bundles = new();

        private Dictionary<string, BadAssetInfo> _guid2Assets = new();

        private Dictionary<string, BadAssetInfo> _name2Assets = new();

        public static void Load( string assetInfoFilePath )
        {
            var file = BadAssetInfoFile.ReadFromFile( assetInfoFilePath );
            instance = new BadAssetLibrary( file );
        }
        
        private BadAssetLibrary( BadAssetInfoFile file )
        {
            this.InitializeBundles( file );
            this.InitializeAssets( file );
        }
        
        public BadBundleInfo GetBundleInfo( string name )
        {
            return _bundles.GetValueOrDefault( name );
        }

        public BadAssetInfo GetAssetInfo( string name )
        {
            return _name2Assets.GetValueOrDefault( name );
        }
        
        public BadAssetInfo GetAssetInfoByGuid( string guid )
        {
            return _guid2Assets.GetValueOrDefault( guid );
        }

        private void InitializeAssets( BadAssetInfoFile file )
        {
            foreach( var pair in file.assets )
            {
                var asset = pair.Value;
                this.InitializeAsset( asset );
            }

            foreach( var pair in file.assets )
            {
                var asset = pair.Value;
                this.ResolveDependencies( asset );
            }
        }

        private void InitializeAsset( BadAssetInfoChunk asset )
        {
            var info = new BadAssetInfo();
            info.name = asset.name;
            info.path = asset.path;
            info.guid = asset.guid;
            info.bundle = this.GetBundleInfo( asset.bundle );
            if( info.bundle == null )
            {
                throw new Exception( $"Cannot find bundle info '{asset.bundle}' for asset '{asset.guid}'" );
            }
            
            _guid2Assets.Add( info.guid, info );
            _name2Assets.Add( info.name, info );
            info.bundle.AddAsset( info );
        }
        
        private void ResolveDependencies( BadAssetInfoChunk asset )
        {
            var info = this.GetAssetInfoByGuid( asset.guid );
            if( info == null )
            {
                throw new Exception( $"Cannot find asset info for {asset.guid}" );
            }
            
            foreach( var dependencyGuid in asset.dependencies )
            {
                var dependencyInfo = this.GetAssetInfoByGuid( dependencyGuid );
                if( dependencyInfo == null )
                {
                    throw new Exception( $"Cannot find dependency info '{dependencyGuid}' for asset '{asset.guid}'" );
                }
                info.dependencies.Add( dependencyInfo );
            }
        }

        private void InitializeBundles( BadAssetInfoFile file )
        {
            foreach( var pair in file.bundles )
            {
                var bundle = pair.Value;
                this.InitializeBundle( bundle );
            }
        }

        private void InitializeBundle( BadBundleInfoChunk bundle )
        {
            var info = new BadBundleInfo( bundle.name, bundle.hash, bundle.location );
            info.state = BadBundleState.Downloaded;
            _bundles.Add( info.name, info );
        }
    }
}