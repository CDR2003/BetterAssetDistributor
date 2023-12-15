using UnityEngine;

namespace RocketPunch.Bad
{
    public class BadVersionInfo
    {
        public const string Filename = "version.json";
        
        public string version;

        public string assetInfoFilePath;

        public static BadVersionInfo ReadFromBytes( byte[] content )
        {
            var json = System.Text.Encoding.UTF8.GetString( content );
            return JsonUtility.FromJson<BadVersionInfo>( json );
        }
    }
}