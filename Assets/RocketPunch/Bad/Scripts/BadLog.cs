namespace RocketPunch.Bad
{
    public static class BadLog
    {
        public static bool enabled = true;
        
        private const string Prefix = "[BAD] ";
        
        public static void Info( string message )
        {
            if( enabled == false )
            {
                return;
            }
            
            UnityEngine.Debug.Log( Prefix + message );
        }
        
        public static void Warning( string message )
        {
            if( enabled == false )
            {
                return;
            }
            
            UnityEngine.Debug.LogWarning( Prefix + message );
        }
        
        public static void Error( string message )
        {
            if( enabled == false )
            {
                return;
            }
            
            UnityEngine.Debug.LogError( Prefix + message );
        }
    }
}