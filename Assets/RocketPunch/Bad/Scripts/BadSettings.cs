using System;
using UnityEngine;

namespace RocketPunch.Bad
{
    [CreateAssetMenu( fileName = "BadSettings", menuName = "BAD/Create Settings" )]
    public class BadSettings : ScriptableObject
    {
        public static BadSettings instance { get; private set; }
        
        [Tooltip( "Should be where version.json is placed." )]
        public string serverUrl;

        [Tooltip( "The folder where you load assets from that already bundled in the build. Should be in StreamingAssets." )]
        public string localAssetPath;

        [Tooltip( "The folder where you download assets from server. Should be relative to Application.persistentDataPath." )]
        public string localDownloadPath;
        
        public static void Load()
        {
            instance = Resources.Load<BadSettings>( "BadSettings" );
            if( instance == null )
            {
                throw new Exception( $"BadSettings cannot be found. Please create a BadSettings asset in Resources folder." );
            }
        }
    }
}