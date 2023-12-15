using System.IO;
using UnityEngine;

namespace RocketPunch.Bad
{
    public static class BadPathHelper
    {
        public static string GetLocalAssetPath( string name )
        {
            var settings = BadSettings.instance;
            return Path.Join( Application.streamingAssetsPath, Path.Join( settings.localAssetPath, name ) );
        }

        public static string GetLocalDownloadPath( string name )
        {
            var settings = BadSettings.instance;
            return Path.Join( Application.persistentDataPath, Path.Join( settings.localDownloadPath, name ) );
        }
        
        public static string GetRemoteAssetPath( string name )
        {
            var settings = BadSettings.instance;
            return Path.Join( settings.serverUrl, name );
        }
    }
}