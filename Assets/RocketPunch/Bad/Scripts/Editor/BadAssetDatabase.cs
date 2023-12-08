using System.Collections.Generic;
using UnityEditor;

namespace RocketPunch.Bad
{
    public static class BadAssetDatabase
    {
        private static Dictionary<string, BadAsset> _guidToAssets = new();
        
        private static Dictionary<string, BadAsset> _pathToAssets = new();
        
        public static BadAsset GetAssetByGuid( string guid )
        {
            return _guidToAssets.GetValueOrDefault( guid );
        }
        
        public static BadAsset GetAssetByPath( string path )
        {
            return _pathToAssets.GetValueOrDefault( path );
        }

        public static BadAsset GetOrAddAssetByGuid( string guid )
        {
            var asset = GetAssetByGuid( guid );
            return asset ?? AddAssetByGuid( guid );
        }

        public static BadAsset GetOrAddAssetByPath( string path )
        {
            var asset = GetAssetByPath( path );
            return asset ?? AddAssetByPath( path );
        }
        
        public static BadAsset AddAssetByGuid( string guid )
        {
            var path = AssetDatabase.GUIDToAssetPath( guid );
            var asset = CreateAsset( guid, path );
            AddAsset( asset );
            return asset;
        }

        public static BadAsset AddAssetByPath( string path )
        {
            var guid = AssetDatabase.AssetPathToGUID( path );
            var asset = CreateAsset( guid, path );
            AddAsset( asset );
            return asset;
        }

        public static void Clear()
        {
            _guidToAssets.Clear();
            _pathToAssets.Clear();
        }

        private static void AddAsset( BadAsset asset )
        {
            _guidToAssets.Add( asset.guid, asset );
            _pathToAssets.Add( asset.path, asset );
            
            CollectDependencies( asset );
        }

        private static void CollectDependencies( BadAsset asset )
        {
            var dependencyPaths = AssetDatabase.GetDependencies( asset.path, false );
            foreach( var dependencyPath in dependencyPaths )
            {
                if( dependencyPath.EndsWith( ".cs" ) )
                {
                    // Skip C# scripts
                    continue;
                }
                
                var dependency = GetOrAddAssetByPath( dependencyPath );
                asset.dependencies.Add( dependency );
                dependency.referencers.Add( asset );
            }
        }

        private static BadAsset CreateAsset( string guid, string path )
        {
            return new BadAsset( guid, path );
        }
    }
}