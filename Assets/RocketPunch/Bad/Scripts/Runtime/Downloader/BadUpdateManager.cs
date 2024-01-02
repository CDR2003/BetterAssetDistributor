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

        private BadDownloadListFile _downloadList;

        public void Initialize()
        {
            BadSettings.Load();
            BadPathHelper.MakeDownloadFolder();
        }

        public void CheckVersion()
        {
            var operation = new BadCheckVersionOperation();
            operation.complete += this.OnCheckVersionCompleted;
            operation.error += this.OnCheckVersionError;
            BadOperationScheduler.instance.EnqueueOperation( operation );
        }

        public BadDownloader StartDownload()
        {
            if( _downloadList == null )
            {
                throw new Exception( "Download list is not generated yet. Please call CheckVersion() first." );
            }
            
            var downloader = new BadDownloader( _downloadList, _remoteVersion );
            downloader.Start();
            return downloader;
        }

        private void OnCheckVersionError( BadOperation operation, string message )
        {
            operation.complete -= this.OnCheckVersionCompleted;
            operation.error -= this.OnCheckVersionError;
            this.error?.Invoke( message );
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
            var operation = new BadGenerateDownloadListOperation( _localVersion.assetInfoFilePath, _remoteVersion.assetInfoFilePath, _remoteVersion );
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
            _downloadList = downloadListOperation.value;
            
            var totalDownloadSize = _downloadList.CalculateTotalDownloadSize();
            var downloadedSize = _downloadList.CalculateDownloadedSize();
            this.versionCheck?.Invoke( new BadVersionCheckResult( _localVersion.version, _remoteVersion.version, totalDownloadSize, downloadedSize ) );
        }
    }
}