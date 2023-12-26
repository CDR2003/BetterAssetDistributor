namespace RocketPunch.Bad
{
    public class BadGenerateDownloadListOperation : BadOperation<BadDownloadListFile>
    {
        private readonly string _localAssetInfoPath;
        
        private readonly string _remoteAssetInfoPath;
        
        private BadReadAssetInfoFileOperation _localAssetInfoOperation;
        
        private BadReadAssetInfoFileOperation _remoteAssetInfoOperation;
        
        public BadGenerateDownloadListOperation( string localAssetInfoPath, string remoteAssetInfoPath )
        {
            _localAssetInfoPath = localAssetInfoPath;
            _remoteAssetInfoPath = remoteAssetInfoPath;
        }
        
        public override void Run()
        {
            var parallelOperation = new BadParallelOperation();
            parallelOperation.complete += this.OnParallelOperationCompleted;
            parallelOperation.error += this.OnParallelOperationError;
            
            _localAssetInfoOperation = new BadReadAssetInfoFileOperation( _localAssetInfoPath );
            _remoteAssetInfoOperation = new BadReadAssetInfoFileOperation( _remoteAssetInfoPath );
            parallelOperation.Add( _localAssetInfoOperation );
            parallelOperation.Add( _remoteAssetInfoOperation );
            parallelOperation.Run();
        }
        
        private void OnParallelOperationCompleted( BadOperation operation )
        {
            operation.complete -= this.OnParallelOperationCompleted;
            operation.error -= this.OnParallelOperationError;
            
            var localAssetInfo = _localAssetInfoOperation.value;
            var remoteAssetInfo = _remoteAssetInfoOperation.value;
            var downloadList = BadDownloadListFile.Create( remoteAssetInfo, localAssetInfo );
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