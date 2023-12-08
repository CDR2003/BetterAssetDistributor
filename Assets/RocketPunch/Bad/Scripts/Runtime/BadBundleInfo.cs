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
            
            this.bundle = AssetBundle.LoadFromFile( "AssetBundles/" + this.name );
            if( this.bundle == null )
            {
                throw new System.Exception( $"Failed to load bundle '{this.name}'" );
            }

            this.state = BadBundleState.Loaded;
            
            BadLog.Info( $"Loaded bundle '{this.name}'" );
        }

        public void AddAsset( BadAssetInfo asset )
        {
            _assets.Add( asset );
        }

        public void Unload()
        {
            Debug.Assert( this.bundle != null );
            
            this.bundle.Unload( true );
            this.bundle = null;
            this.state = BadBundleState.Downloaded;
            
            BadLog.Info( $"Unloaded bundle '{this.name}'" );
        }

        public override string ToString()
        {
            return $"Bundle: {this.name}";
        }
    }
}