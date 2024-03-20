namespace RocketPunch.Bad
{
    public class BadGenerateDownloadListOperation : BadOperation<BadDownloadListFile>
    {
        private readonly string _localAssetInfoPath;
        
        private readonly string _remoteAssetInfoPath;

        private readonly BadVersionInfo _versionInfo;
        
        private BadReadFileOperation _readDownloadListOperation;
        
        private BadReadAssetInfoFileOperation _localAssetInfoOperation;
        
        private BadReadAssetInfoFileOperation _remoteAssetInfoOperation;

        private BadDownloadListFile _downloadList;
        
        public BadGenerateDownloadListOperation( string localAssetInfoPath, string remoteAssetInfoPath, BadVersionInfo versionInfo )
        {
            _localAssetInfoPath = localAssetInfoPath;
            _remoteAssetInfoPath = remoteAssetInfoPath;
            _versionInfo = versionInfo;
        }
        
        public override void Run()
        {
            var downloadListPath = BadPathHelper.GetLocalDownloadPath( BadDownloadListFile.Filename );
            _readDownloadListOperation = new BadReadFileOperation( downloadListPath );
            _readDownloadListOperation.complete += this.OnDownloadListRead;
            _readDownloadListOperation.error += this.OnDownloadListReadError;
            
            var parallelOperation = new BadParallelOperation();
            parallelOperation.complete += this.OnParallelOperationCompleted;
            parallelOperation.error += this.OnParallelOperationError;
            
            _localAssetInfoOperation = new BadReadAssetInfoFileOperation( _localAssetInfoPath );
            _remoteAssetInfoOperation = new BadReadAssetInfoFileOperation( _remoteAssetInfoPath );
            parallelOperation.Add( _readDownloadListOperation );
            parallelOperation.Add( _localAssetInfoOperation );
            parallelOperation.Add( _remoteAssetInfoOperation );
            parallelOperation.Run();
        }
        
        private void OnDownloadListRead( BadOperation operation )
        {
            operation.complete -= this.OnDownloadListRead;
            operation.error -= this.OnDownloadListReadError;

            _downloadList = BadDownloadListFile.ReadFromBytes( _readDownloadListOperation.value );
        }
        
        private void OnDownloadListReadError( BadOperation operation, string message )
        {
            operation.complete -= this.OnDownloadListRead;
            operation.error -= this.OnDownloadListReadError;

            _downloadList = null;
        }
        
        private void OnParallelOperationCompleted( BadOperation operation )
        {
            operation.complete -= this.OnParallelOperationCompleted;
            operation.error -= this.OnParallelOperationError;
            
            var localAssetInfo = _localAssetInfoOperation.value;
            var remoteAssetInfo = _remoteAssetInfoOperation.value;
            if( _downloadList != null && _downloadList.version == _versionInfo.version )
            {
                // Already downloaded some files
                _downloadList.oldInfo = localAssetInfo;
                _downloadList.newInfo = remoteAssetInfo;
                this.Complete( _downloadList );
                return;
            }
            
            var downloadList = BadDownloadListFile.Create( remoteAssetInfo, localAssetInfo, _versionInfo.version );
            this.Complete( downloadList );
        }
        
        private void OnParallelOperationError( BadOperation operation, string message )
        {
            operation.complete -= this.OnParallelOperationCompleted;
            operation.error -= this.OnParallelOperationError;
            this.Error( "Generating download list failed.\n" + message );
        }
    }
}