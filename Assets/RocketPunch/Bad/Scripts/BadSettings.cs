using System;
using UnityEngine;

namespace RocketPunch.Bad
{
    [CreateAssetMenu( fileName = "BadSettings", menuName = "BAD/Create Settings" )]
    public class BadSettings : ScriptableObject
    {
        public static BadSettings instance
        {
            get
            {
                if( !_instance )
                {
                    Load();
                }
                return _instance;
            }
        }
        
        private static BadSettings _instance;

        [Tooltip( "The folder where asset bundle build is placed. Just a temporary folder. Should be relative to project folder." )]
        public string buildPath;
        
        [Tooltip( "Should be where version.json is placed." )]
        public string serverUrl;

        [Tooltip( "The folder where you load assets from that already bundled in the build. Should be in StreamingAssets." )]
        public string localAssetPath;

        [Tooltip( "The folder where you download assets from server. Should be relative to Application.persistentDataPath." )]
        public string localDownloadPath;

        [Tooltip( "The folder where remote build is placed. Should be relative to project folder." )]
        public string remoteBuildPath;
        
        public BadVersion version;
        
        public static void Load()
        {
            _instance = Resources.Load<BadSettings>( "BadSettings" );
            if( !_instance )
            {
                throw new Exception( $"BadSettings cannot be found. Please create a BadSettings asset in Resources folder." );
            }
        }
    }
}