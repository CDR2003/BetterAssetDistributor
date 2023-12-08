using System.Collections.Generic;
using UnityEngine;

namespace RocketPunch.Bad
{
    public static class BadLoadTaskCreator
    {
        public static List<BadSyncLoadTask> CreateSyncLoadTasks( string name )
        {
            var assetInfo = BadAssetLibrary.instance.GetAssetInfo( name );
            if( assetInfo == null )
            {
                throw new System.Exception( $"Cannot find asset info for asset '{name}'" );
            }
            
            var tasks = new List<BadSyncLoadTask>();
            EnqueueAssetSyncLoadTask( assetInfo, tasks );
            return tasks;
        }
        
        public static List<BadSyncLoadTask> CreateSyncUnloadTasks( Object obj )
        {
            var tasks = new List<BadSyncLoadTask>();
            tasks.Add( new BadAssetSyncUnloadTask( obj ) );
            return tasks;
        }

        public static List<BadAsyncLoadTask> CreateAsyncLoadTasks( string name )
        {
            var assetInfo = BadAssetLibrary.instance.GetAssetInfo( name );
            if( assetInfo == null )
            {
                throw new System.Exception( $"Cannot find asset info for asset '{name}'" );
            }
            
            var tasks = new List<BadAsyncLoadTask>();
            EnqueueAssetAsyncLoadTask( assetInfo, tasks );
            return tasks;
        }

        private static void EnqueueAssetSyncLoadTask( string guid, List<BadSyncLoadTask> tasks )
        {
            var assetInfo = BadAssetLibrary.instance.GetAssetInfoByGuid( guid );
            if( assetInfo == null )
            {
                throw new System.Exception( $"Cannot find asset info for asset '{guid}'" );
            }

            EnqueueAssetSyncLoadTask( assetInfo, tasks );
        }

        private static void EnqueueAssetSyncLoadTask( BadAssetInfo assetInfo, List<BadSyncLoadTask> tasks )
        {
            foreach( var dependency in assetInfo.dependencies )
            {
                EnqueueAssetSyncLoadTask( dependency.guid, tasks );
            }
            
            EnqueueBundleSyncLoadTask( assetInfo.bundle.name, tasks );

            var task = new BadAssetSyncLoadTask( assetInfo );
            tasks.Add( task );
        }

        private static void EnqueueBundleSyncLoadTask( string name, List<BadSyncLoadTask> tasks )
        {
            var bundleInfo = BadAssetLibrary.instance.GetBundleInfo( name );
            if( bundleInfo == null )
            {
                throw new System.Exception( $"Cannot find bundle info for bundle '{name}'" );
            }

            var task = new BadBundleSyncLoadTask( bundleInfo );
            tasks.Add( task );
        }
        
        private static void EnqueueAssetAsyncLoadTask( string guid, List<BadAsyncLoadTask> tasks )
        {
            var assetInfo = BadAssetLibrary.instance.GetAssetInfoByGuid( guid );
            if( assetInfo == null )
            {
                throw new System.Exception( $"Cannot find asset info for asset '{guid}'" );
            }

            EnqueueAssetAsyncLoadTask( assetInfo, tasks );
        }
        
        private static void EnqueueAssetAsyncLoadTask( BadAssetInfo assetInfo, List<BadAsyncLoadTask> tasks )
        {
            foreach( var dependency in assetInfo.dependencies )
            {
                EnqueueAssetAsyncLoadTask( dependency.guid, tasks );
            }
            
            EnqueueBundleAsyncLoadTask( assetInfo.bundle.name, tasks );

            var task = new BadAssetAsyncLoadTask( assetInfo );
            tasks.Add( task );
        }
        
        private static void EnqueueBundleAsyncLoadTask( string name, List<BadAsyncLoadTask> tasks )
        {
            var bundleInfo = BadAssetLibrary.instance.GetBundleInfo( name );
            if( bundleInfo == null )
            {
                throw new System.Exception( $"Cannot find bundle info for bundle '{name}'" );
            }

            var task = new BadBundleAsyncLoadTask( bundleInfo );
            tasks.Add( task );
        }
    }
}