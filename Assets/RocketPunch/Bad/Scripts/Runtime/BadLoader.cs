using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketPunch.Bad
{
    public static class BadLoader
    {
        private static Queue<BadAsyncLoadTask> _pendingTasks = new();

        private static BadAsyncLoadTask _currentTask;
        
        public static T Load<T>( string name ) where T : UnityEngine.Object
        {
            var tasks = BadLoadTaskCreator.CreateSyncLoadTasks( name );
            if( tasks.Count == 0 )
            {
                throw new Exception( $"No tasks created for loading asset '{name}'" );
            }

            RunSyncTasks( tasks );

            var asset = BadAssetLibrary.instance.GetAssetInfo( name );
            return asset.loadedInfo.obj as T;
        }

        public static void Unload( UnityEngine.Object obj )
        {
            var tasks = BadLoadTaskCreator.CreateSyncUnloadTasks( obj );
            if( tasks.Count == 0 )
            {
                throw new Exception( $"No tasks created for unloading asset '{obj.name}'" );
            }

            RunSyncTasks( tasks );
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