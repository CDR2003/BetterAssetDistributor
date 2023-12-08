using System;
using System.Collections.Generic;
using UnityEngine;

namespace RocketPunch.Bad
{
    public class BadAssetInfo
    {
        public string name;
        
        public string path;
        
        public string guid;
        
        public List<BadAssetInfo> dependencies = new();
        
        public BadBundleInfo bundle;

        public BadLoadedAssetInfo loadedInfo;

        public void Load()
        {
            if( this.loadedInfo == null )
            {
                this.DoLoad();
            }
            
            this.loadedInfo.referenceCount++;
        }
        
        public BadAssetLoadOperation LoadAsync()
        {
            if( this.loadedInfo != null )
            {
                this.loadedInfo.referenceCount++;
                return null;
            }

            var bundle = this.bundle.bundle;
            Debug.Assert( bundle );
            
            var request = bundle.LoadAssetAsync( this.guid );
            return new BadAssetLoadOperation( this, request );
        }

        public void Unload()
        {
            Debug.Assert( this.loadedInfo != null );
            
            this.loadedInfo.referenceCount--;
            if( this.loadedInfo.referenceCount == 0 )
            {
                this.DoUnload();
            }
            
            this.RemoveDependencies();
        }

        public void UnloadAsync()
        {
            Debug.Assert( this.loadedInfo != null );
            
            this.loadedInfo.referenceCount--;
            if( this.loadedInfo.referenceCount == 0 )
            {
                this.DoUnloadAsync();
            }
            
            this.RemoveDependenciesAsync();
        }

        public override string ToString()
        {
            return $"Asset: {this.name} ({this.guid}) @ '{this.bundle.name}'";
        }

        private void DoLoad()
        {
            if( this.bundle.bundle == null )
            {
                throw new Exception( $"Bundle '{this.bundle.name}' has not been loaded for asset '{this.guid}'" );
            }

            var obj = bundle.bundle.LoadAsset( this.guid );
            if( obj == null )
            {
                throw new Exception( $"Failed to load asset '{this.name}' from bundle '{this.bundle.name}'" );
            }
            
            this.loadedInfo = new BadLoadedAssetInfo( this, obj );
            BadLoadedAssetLibrary.Add( this.loadedInfo );
            
            BadLog.Info( $"[SYNC] Loaded asset '{this.name}' ({this.guid}) from bundle '{this.bundle.name}'" );
        }

        private void DoUnload()
        {
            BadLoadedAssetLibrary.Remove( this.loadedInfo );
            this.loadedInfo = null;
            
            BadLog.Info( $"[SYNC] Unloaded asset '{this.name}' ({this.guid}) from bundle '{this.bundle.name}'" );
            
            this.TryUnloadBundle();
        }
        
        private void DoUnloadAsync()
        {
            BadLoadedAssetLibrary.Remove( this.loadedInfo );
            this.loadedInfo = null;
            
            BadLog.Info( $"[ASYNC] Unloaded asset '{this.name}' ({this.guid}) from bundle '{this.bundle.name}'" );
            
            this.TryUnloadBundleAsync();
        }
        
        private void TryUnloadBundle()
        {
            if( this.bundle.hasLoadedAssets == false )
            {
                this.bundle.Unload();
            }
        }
        
        private void TryUnloadBundleAsync()
        {
            if( this.bundle.hasLoadedAssets == false )
            {
                this.bundle.UnloadAsync();
            }
        }
        
        private void RemoveDependencies()
        {
            foreach( var dependency in this.dependencies )
            {
                dependency.Unload();
            }
        }
        
        private void RemoveDependenciesAsync()
        {
            foreach( var dependency in this.dependencies )
            {
                dependency.UnloadAsync();
            }
        }
    }
}