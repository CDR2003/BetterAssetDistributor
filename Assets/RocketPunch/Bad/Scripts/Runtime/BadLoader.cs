using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketPunch.Bad
{
    public static class BadLoader
    {
        public static T Load<T>( string name ) where T : UnityEngine.Object
        {
            if( string.IsNullOrEmpty( name ) )
            {
                throw new Exception( $"Name is null or empty" );
            }
            
            var tasks = BadLoadTaskCreator.CreateSyncLoadTasks( name );
            if( tasks.Count == 0 )
            {
                throw new Exception( $"No tasks created for loading asset '{name}'" );
            }

            RunSyncTasks( tasks );

            var asset = BadAssetLibrary.instance.GetAssetInfo( name );
            return asset.loadedInfo.obj as T;
        }

        public static BadAssetAsyncLoadTask LoadAsync<T>( string name ) where T : UnityEngine.Object
        {
            if( string.IsNullOrEmpty( name ) )
            {
                throw new Exception( $"Name is null or empty" );
            }
            
            var tasks = BadLoadTaskCreator.CreateAsyncLoadTasks( name );
            if( tasks.Count == 0 )
            {
                throw new Exception( $"No tasks created for loading asset '{name}'" );
            }

            BadTaskScheduler.instance.EnqueueTasks( tasks );

            var lastTask = tasks[^1] as BadAssetAsyncLoadTask;
            Debug.Assert( lastTask != null );
            
            return lastTask;
        }

        public static void Unload( UnityEngine.Object obj )
        {
            if( !obj )
            {
                throw new Exception( $"Object is null" );
            }
            
            var tasks = BadLoadTaskCreator.CreateSyncUnloadTasks( obj );
            if( tasks.Count == 0 )
            {
                throw new Exception( $"No tasks created for unloading asset '{obj.name}'" );
            }

            RunSyncTasks( tasks );
        }

        public static void UnloadAsync( UnityEngine.Object obj )
        {
            var loadedAsset = BadLoadedAssetLibrary.Get( obj );
            if( loadedAsset == null )
            {
                throw new Exception( $"Cannot find asset '{obj.name}' ({obj.GetType()}) in loaded asset library" );
            }
            
            loadedAsset.asset.UnloadAsync();
        }
        
        private static void RunSyncTasks( List<BadSyncLoadTask> tasks )
        {
            foreach( var task in tasks )
            {
                task.Run();
            }
        }
    }
}