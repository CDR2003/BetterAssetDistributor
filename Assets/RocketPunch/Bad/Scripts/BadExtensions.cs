namespace RocketPunch.Bad
{
    public static class BadExtensions
    {
        public static string ToFileSizeString( this int size )
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = (double)size;
            int order = 0;
            while( len >= 1024 && order < sizes.Length - 1 )
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
    }
}