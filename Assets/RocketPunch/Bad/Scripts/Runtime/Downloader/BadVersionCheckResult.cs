namespace RocketPunch.Bad
{
    public class BadVersionCheckResult
    {
        public readonly string remoteVersion;
        
        public readonly string localVersion;

        public readonly bool needsUpdate;
        
        public readonly int totalDownloadSize;
        
        public readonly int downloadedSize;

        public BadVersionCheckResult( string version )
        {
            this.remoteVersion = version;
            this.localVersion = version;
            this.needsUpdate = false;
            this.totalDownloadSize = 0;
            this.downloadedSize = 0;
        }

        public BadVersionCheckResult( string localVersion, string remoteVersion, int totalDownloadSize,
            int downloadedSize )
        {
            this.localVersion = localVersion;
            this.remoteVersion = remoteVersion;
            this.needsUpdate = true;
            this.totalDownloadSize = totalDownloadSize;
            this.downloadedSize = downloadedSize;
        }
    }
}