using System.Collections.Generic;
using UnityEngine;

namespace RocketPunch.Bad
{
    public static class BadLoadedAssetLibrary
    {
        private static Dictionary<Object, BadLoadedAssetInfo> _loadedAssets = new();
        
        public static BadLoadedAssetInfo Get( Object obj )
        {
            return _loadedAssets.GetValueOrDefault( obj );
        }
        
        public static void Add( BadLoadedAssetInfo loadedAsset )
        {
            _loadedAssets.Add( loadedAsset.obj, loadedAsset );
        }

        public static void Remove( BadLoadedAssetInfo loadedAsset )
        {
            var removed = _loadedAssets.Remove( loadedAsset.obj );
            Debug.Assert( removed );
        }
    }
}