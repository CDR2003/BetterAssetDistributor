using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RocketPunch.Bad
{
    public static class BadBuilderTest
    {
        [MenuItem( "BAD/Build" )]
        public static void Build()
        {
            BadAssetDatabase.Clear();
            
            var groups = GetAssetGroups();
            var builder = new BadFullBuilder();
            builder.Build( groups );
        }

        [MenuItem( "BAD/Check Modifications" )]
        public static void CheckModifications()
        {
            BadAssetDatabase.Clear();
            
            var assetStateFilePath = EditorUtility.OpenFilePanel( "Select Asset State File", "AssetBundles", "bad" );
            if( assetStateFilePath.Length == 0 )
            {
                return;
            }
            
            AssetDatabase.SaveAssets();
            
            var groups = GetAssetGroups();
            var assetState = BadAssetStateFile.ReadFromFile( assetStateFilePath );
            var checker = new BadModificationChecker( assetState );
            checker.CheckForModifiedAssets( groups );

            var modifiedAssets = checker.modifiedAssets;
            Debug.Log( "Modified Assets: Total " + modifiedAssets.Count + " assets" );
            foreach( var asset in modifiedAssets )
            {
                Debug.Log( asset.path );
            }
        }
        
        [MenuItem( "BAD/Build Update" )]
        public static void BuildUpdate()
        {
            BadAssetDatabase.Clear();
            
            var assetStateFilePath = EditorUtility.OpenFilePanel( "Select Asset State File", "AssetBundles", "bad" );
            if( assetStateFilePath.Length == 0 )
            {
                return;
            }
            
            AssetDatabase.SaveAssets();
            
            var groups = GetAssetGroups();
            var builder = new BadUpdateBuilder( assetStateFilePath );
            builder.Build( groups );
        }

        [MenuItem( "BAD/Clear All Related Directories" )]
        public static void ClearAllRelatedDirectories()
        {
            ClearDirectory( BadSettings.instance.buildPath );
            ClearDirectory( BadSettings.instance.remoteBuildPath );
            ClearDirectory( Path.Join( Application.streamingAssetsPath, BadSettings.instance.localAssetPath ) );
            ClearDirectory( Path.Join( Application.persistentDataPath, BadSettings.instance.localDownloadPath ) );
        }

        private static void ClearDirectory( string path )
        {
            if( !Directory.Exists( path ) )
            {
                return;
            }
            
            var files = Directory.GetFiles( path );
            foreach( var file in files )
            {
                File.Delete( file );
            }
            
            var directories = Directory.GetDirectories( path );
            foreach( var directory in directories )
            {
                Directory.Delete( directory, true );
            }
        }

        [MenuItem( "BAD/Unload All" )]
        public static void UnloadAll()
        {
            AssetBundle.UnloadAllAssetBundles( true );
        }

        private static List<BadAssetGroup> GetAssetGroups()
        {
            var assetGroupDefs = AssetDatabase.FindAssets( "t:BadAssetGroupDef" ).Select( AssetDatabase.GUIDToAssetPath ).Select( AssetDatabase.LoadAssetAtPath<BadAssetGroupDef> ).ToList();
            var assetGroups = new List<BadAssetGroup>();
            foreach( var def in assetGroupDefs )
            {
                var assetGroup = new BadAssetGroup( def.name, def.location );
                foreach( var asset in def.assets )
                {
                    var assetPath = AssetDatabase.GetAssetPath( asset );
                    var badAsset = BadAssetDatabase.GetOrAddAssetByPath( assetPath );
                    assetGroup.assets.Add( badAsset );
                }
                assetGroups.Add( assetGroup );
            }
            return assetGroups;
        }
    }
}