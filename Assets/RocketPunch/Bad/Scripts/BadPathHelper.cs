namespace RocketPunch.Bad
{
    public static class BadPathHelper
    {
        public static string GetLocalAssetPath( string name )
        {
            return $"AssetBundles/{name}";
        }
    }
}