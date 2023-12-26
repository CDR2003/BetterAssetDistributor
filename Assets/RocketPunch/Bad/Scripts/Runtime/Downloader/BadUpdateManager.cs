using System;

namespace RocketPunch.Bad
{
    public class BadUpdateManager
    {
        public static BadUpdateManager instance { get; } = new BadUpdateManager();
        
        public event Action<BadVersionCheckResult> versionCheck;

        public event Action<string> error;
        
        private BadVersionInfo _localVersion;
        
        private BadVersionInfo _remoteVersion;
        
        public void CheckVersion()
        {
            var operation = new BadCheckVersionOperation();
            operation.complete += this.OnCheckVersionCompleted;
            operation.error += this.OnCheckVersionError;
            BadOperationScheduler.instance.EnqueueOperation( operation );
        }

        private void OnCheckVersionError( BadOperation operation, string message )
        {
            operation.complete -= this.OnCheckVersionCompleted;
            operation.error -= this.OnCheckVersionError;
        }

        private void OnCheckVersionCompleted( BadOperation operation )
        {
            operation.complete -= this.OnCheckVersionCompleted;
            operation.error -= this.OnCheckVersionError;
            
            var checkVersionOperation = (BadCheckVersionOperation)operation;
            _localVersion = checkVersionOperation.localVersion;
            _remoteVersion = checkVersionOperation.remoteVersion;
            if( _localVersion.version == _remoteVersion.version )
            {
                this.versionCheck?.Invoke( new BadVersionCheckResult( _remoteVersion.version ) );
                return;
            }
            
            this.GenerateDownloadList();
        }
        
        private void GenerateDownloadList()
        {
            var operation = new BadGenerateDownloadListOperation( _localVersion.assetInfoFilePath, _remoteVersion.assetInfoFilePath );
            operation.complete += this.OnGenerateDownloadListCompleted;
            operation.error += this.OnGenerateDownloadListError;
            operation.Run();
        }
        
        private void OnGenerateDownloadListError( BadOperation operation, string message )
        {
            operation.complete -= this.OnGenerateDownloadListCompleted;
            operation.error -= this.OnGenerateDownloadListError;
            this.error?.Invoke( message );
        }
        
        private void OnGenerateDownloadListCompleted( BadOperation operation )
        {
            operation.complete -= this.OnGenerateDownloadListCompleted;
            operation.error -= this.OnGenerateDownloadListError;

            var downloadListOperation = (BadGenerateDownloadListOperation)operation;
            var downloadList = downloadListOperation.value;
            var totalDownloadSize = downloadList.CalculateTotalDownloadSize();
            var downloadedSize = downloadList.CalculateDownloadedSize();
            this.versionCheck?.Invoke( new BadVersionCheckResult( _localVersion.version, _remoteVersion.version, totalDownloadSize, downloadedSize ) );
        }
    }
}