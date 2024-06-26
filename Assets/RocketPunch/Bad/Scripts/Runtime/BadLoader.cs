﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RocketPunch.Bad
{
    public static class BadLoader
    {
        public static event Action initializeComplete; 
        
        public static void Initialize()
        {
            var operation = new BadCheckLocalVersionOperation();
            operation.complete += OnLocalVersionLoaded;
            operation.error += OnLoadVersionError;
            operation.Run();
        }

        private static void OnLoadVersionError( BadOperation operation, string message )
        {
            operation.complete -= OnLocalVersionLoaded;
            operation.error -= OnLoadVersionError;

            throw new Exception( message );
        }

        private static void OnLocalVersionLoaded( BadOperation operation )
        {
            operation.complete -= OnLocalVersionLoaded;
            operation.error -= OnLoadVersionError;
            
            var localVersion = (BadCheckLocalVersionOperation)operation;
            var versionInfo = localVersion.value;
            BadAssetLibrary.Load( versionInfo.assetInfoFilePath );
            
            initializeComplete?.Invoke();
        }

        public static T Load<T>( string name ) where T : UnityEngine.Object
        {
            var asset = BadAssetLibrary.instance.GetAssetInfo( name );
            asset.Load();
            return asset.loadedInfo.obj as T;
        }

        public static T LoadByGuid<T>(string key) where T : UnityEngine.Object
        {
            var asset = BadAssetLibrary.instance.GetAssetInfoByGuid(key);
            asset.Load();
            return asset.loadedInfo.obj as T;
        }

        public static BadLoadAssetOperation<T> LoadAsync<T>( string name ) where T : UnityEngine.Object
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

            var operation = asset.LoadAsync<T>();
            BadOperationScheduler.instance.EnqueueOperation( operation );

            return operation;
        }
        
        public static BadLoadAssetOperation<T> LoadAsyncByGuid<T>( string key ) where T : UnityEngine.Object
        {
            if( string.IsNullOrEmpty( key ) )
            {
                throw new Exception( $"Key is null or empty" );
            }
            
            var asset = BadAssetLibrary.instance.GetAssetInfoByGuid( key );
            if( asset == null )
            {
                throw new Exception( $"Cannot find asset '{key}'" );
            }

            var operation = asset.LoadAsync<T>();
            BadOperationScheduler.instance.EnqueueOperation( operation );

            return operation;
        }
        
        public static BadLoadSceneOperation LoadSceneAsync(string guid, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if( string.IsNullOrEmpty( guid ) )
            {
                throw new Exception( $"Guid is null or empty" );
            }
            
            var asset = BadAssetLibrary.instance.GetAssetInfoByGuid( guid );
            if( asset == null )
            {
                throw new Exception( $"Cannot find asset '{guid}'" );
            }

            var operation = asset.LoadSceneAsync(loadSceneMode);
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

        public static BadUnloadSceneOperation UnloadSceneAsync(BadLoadSceneOperation badLoadSceneOperation)
        {
            var operation = badLoadSceneOperation.UnloadAsync();
            BadOperationScheduler.instance.EnqueueOperation( operation );
            return operation;
        }
    }
}