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
        
        public bool hasLoaded => this.bundle != null;
        
        private readonly HashSet<BadAssetInfo> _assets = new();

        public BadBundleInfo( string name, byte[] hash )
        {
            this.name = name;
            this.hash = hash;
        }
        
        public void Load()
        {
            if( this.bundle )
            {
                return;
            }
            
            var path = BadPathHelper.GetLocalAssetPath( this.name );
            this.bundle = AssetBundle.LoadFromFile( path );
            if( !this.bundle )
            {
                throw new System.Exception( $"Failed to load bundle '{this.name}'" );
            }

            this.state = BadBundleState.Loaded;
            
            BadLog.Info( $"[SYNC] Loaded bundle '{this.name}'" );
        }

        public BadLoadBundleOperation LoadAsync()
        {
            return new BadLoadBundleOperation( this );
        }

        public Object LoadAsset( string guid )
        {
            Debug.Assert( this.hasLoaded );
            return this.bundle.LoadAsset( guid );
        }

        public void TryUnload()
        {
            if( this.hasLoadedAssets )
            {
                return;
            }
            
            this.Unload();
        }

        public void Unload()
        {
            Debug.Assert( this.bundle );
            
            this.bundle.Unload( true );
            this.bundle = null;
            this.state = BadBundleState.Downloaded;
            
            BadLog.Info( $"[SYNC] Unloaded bundle '{this.name}'" );
        }

        public BadUnloadBundleOperation UnloadAsync()
        {
            return new BadUnloadBundleOperation( this );
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