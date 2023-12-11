using System;
using System.Collections.Generic;
using RocketPunch.Bad.Operations;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketPunch.Bad
{
    public static class BadLoader
    {
        public static T Load<T>( string name ) where T : UnityEngine.Object
        {
            var asset = BadAssetLibrary.instance.GetAssetInfo( name );
            asset.Load();
            return asset.loadedInfo.obj as T;
        }

        public static BadLoadAssetOperation LoadAsync<T>( string name ) where T : UnityEngine.Object
        {
            if( string.IsNullOrEmpty( name ) )
            {
                throw new Exception( $"Name is null or empty" );
            }
            
            var asset = BadAssetLibrary.instance.GetAssetInfo( name );
            if( asset == null )
            {
                throw new Exception( $"Cannot find asset '{name}'" );
            }

            var operation = asset.LoadAsync();
            BadOperationScheduler.instance.EnqueueOperation( operation );

            return operation;
        }

        public static void Unload( UnityEngine.Object obj )
        {
            if( !obj )
            {
                throw new Exception( $"Object is null" );
            }

            var loadedAsset = BadLoadedAssetLibrary.Get( obj );
            if( loadedAsset == null )
            {
                throw new Exception( $"Cannot find asset '{obj.name}' ({obj.GetType()}) in loaded asset library" );
            }

            var asset = loadedAsset.asset;
            asset.Unload();
        }

        public static BadOperation UnloadAsync( UnityEngine.Object obj )
        {
            var loadedAsset = BadLoadedAssetLibrary.Get( obj );
            if( loadedAsset == null )
            {
                throw new Exception( $"Cannot find asset '{obj.name}' ({obj.GetType()}) in loaded asset library" );
            }

            var asset = loadedAsset.asset;
            var operation = asset.UnloadAsync();
            BadOperationScheduler.instance.EnqueueOperation( operation );
            
            return operation;
        }
    }
}