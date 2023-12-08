using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Player;
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
            var builder = new BadBuilder();
            builder.Build( groups, "AssetBundles" );
        }

        [MenuItem( "BAD/Check Modifications" )]
        public static void CheckModifications()
        {
            BadAssetDatabase.Clear();
            
            var groups = GetAssetGroups();
            var modifiedAssets = BadModificationChecker.CheckForModifiedAssets( groups, "AssetBundles/asset_state_20231201174756.bad" );
            foreach( var asset in modifiedAssets )
            {
                Debug.Log( asset.path );
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
                var assetGroup = new BadAssetGroup( def.name );
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