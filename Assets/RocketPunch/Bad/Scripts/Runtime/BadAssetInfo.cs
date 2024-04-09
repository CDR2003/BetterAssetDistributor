using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public bool hasLoaded => this.loadedInfo != null;

        public void Load()
        {
            if( this.hasLoaded == false )
            {
                this.DoLoad();
            }
            
            this.loadedInfo.referenceCount++;
        }
        
        public BadLoadAssetOperation<T> LoadAsync<T>() where T : UnityEngine.Object
        {
            return new BadLoadAssetOperation<T>( this );
        }

        public BadLoadSceneOperation LoadSceneAsync(LoadSceneMode loadSceneMode)
        {
            return new BadLoadSceneOperation( this, loadSceneMode );
        }

        public void Unload()
        {
            Debug.Assert( this.loadedInfo != null );
            
            this.loadedInfo.referenceCount--;
            if( this.loadedInfo.referenceCount == 0 )
            {
                this.DoUnload();
            }
            
            this.UnloadDependencies();
        }

        public BadUnloadAssetOperation UnloadAsync()
        {
            Debug.Assert( this.loadedInfo != null );
            return new BadUnloadAssetOperation( this );
        }

        public BadUnloadSceneOperation UnloadSceneAsync()
        {
            return new BadUnloadSceneOperation( this );
        }

        public override string ToString()
        {
            return $"Asset: {this.name} ({this.guid}) @ '{this.bundle.name}'";
        }

        private void DoLoad()
        {
            if( this.bundle.hasLoaded == false )
            {
                this.bundle.Load();
            }

            var obj = bundle.LoadAsset( this.guid );
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
            
            this.bundle.TryUnload();
        }
        
        private void UnloadDependencies()
        {
            foreach( var dependency in this.dependencies )
            {
                dependency.Unload();
            }
        }
    }
}