using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RocketPunch.Bad
{
    public class BadBundleInfo
    {
        public readonly string name;
        
        public readonly byte[] hash;

        public BadBundleState state;

        public AssetBundle bundle;

        public bool hasLoadedAssets => _assets.Any( a => a.loadedInfo?.referenceCount > 0 );
        
        private readonly HashSet<BadAssetInfo> _assets = new();

        public BadBundleInfo( string name, byte[] hash )
        {
            this.name = name;
            this.hash = hash;
        }
        
        public void Load()
        {
            if( this.bundle != null )
            {
                return;
            }
            
            var path = BadPathHelper.GetAssetBundlePath( this.name );
            this.bundle = AssetBundle.LoadFromFile( path );
            if( this.bundle == null )
            {
                throw new System.Exception( $"Failed to load bundle '{this.name}'" );
            }

            this.state = BadBundleState.Loaded;
            
            BadLog.Info( $"[SYNC] Loaded bundle '{this.name}'" );
        }

        public BadBundleLoadOperation LoadAsync()
        {
            if( this.bundle != null )
            {
                return null;
            }

            this.state = BadBundleState.Loading;
            
            var path = BadPathHelper.GetAssetBundlePath( this.name );
            var request = AssetBundle.LoadFromFileAsync( path );
            return new BadBundleLoadOperation( this, request );
        }

        public void Unload()
        {
            Debug.Assert( this.bundle != null );
            
            this.bundle.Unload( true );
            this.bundle = null;
            this.state = BadBundleState.Downloaded;
            
            BadLog.Info( $"[SYNC] Unloaded bundle '{this.name}'" );
        }

        public BadBundleUnloadOperation UnloadAsync()
        {
            this.state = BadBundleState.Unloading;
            
            var request = this.bundle.UnloadAsync( true );
            return new BadBundleUnloadOperation( this, request );
        }

        public void AddAsset( BadAssetInfo asset )
        {
            _assets.Add( asset );
        }

        public override string ToString()
        {
            return $"Bundle: {this.name}";
        }
    }
}