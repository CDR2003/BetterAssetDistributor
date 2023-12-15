namespace RocketPunch.Bad
{
    public class BadCheckLocalVersionOperation : BadOperation<BadVersionInfo>
    {
        private BadReadFileOperation _checkDownloadFolderOperation;
        
        private BadReadFileOperation _checkLocalAssetOperation;
        
        public override void Run()
        {
            _checkDownloadFolderOperation = new BadReadFileOperation( BadPathHelper.GetLocalDownloadPath( BadVersionInfo.Filename ) );
            _checkDownloadFolderOperation.complete += this.OnCheckDownloadFolderOperationCompleted;
            _checkDownloadFolderOperation.error += this.OnCheckDownloadFolderOperationError;
            _checkDownloadFolderOperation.Run();
        }

        private void OnCheckDownloadFolderOperationCompleted( BadOperation operation )
        {
            _checkDownloadFolderOperation.complete -= this.OnCheckDownloadFolderOperationCompleted;
            _checkDownloadFolderOperation.error -= this.OnCheckDownloadFolderOperationError;

            var versionInfo = BadVersionInfo.ReadFromBytes( _checkDownloadFolderOperation.value );
            versionInfo.assetInfoFilePath = BadPathHelper.GetLocalDownloadPath( versionInfo.assetInfoFilePath );
            this.Complete( versionInfo );
        }

        private void OnCheckDownloadFolderOperationError( BadOperation operation, string message )
        {
            _checkDownloadFolderOperation.complete -= this.OnCheckDownloadFolderOperationCompleted;
            _checkDownloadFolderOperation.error -= this.OnCheckDownloadFolderOperationError;
            
            // Download folder does not have version info file in it, check out local asset folder.
            _checkLocalAssetOperation = new BadReadFileOperation( BadPathHelper.GetLocalAssetPath( BadVersionInfo.Filename ) );
            _checkLocalAssetOperation.complete += this.OnCheckLocalAssetOperationCompleted;
            _checkLocalAssetOperation.error += this.OnCheckLocalAssetOperationError;
            _checkLocalAssetOperation.Run();
        }

        private void OnCheckLocalAssetOperationCompleted( BadOperation operation )
        {
            _checkLocalAssetOperation.complete -= this.OnCheckLocalAssetOperationCompleted;
            _checkLocalAssetOperation.error -= this.OnCheckLocalAssetOperationError;

            var versionInfo = BadVersionInfo.ReadFromBytes( _checkLocalAssetOperation.value );
            versionInfo.assetInfoFilePath = BadPathHelper.GetLocalAssetPath( versionInfo.assetInfoFilePath );
            this.Complete( versionInfo );
        }

        private void OnCheckLocalAssetOperationError( BadOperation operation, string message )
        {
            _checkLocalAssetOperation.complete -= this.OnCheckLocalAssetOperationCompleted;
            _checkLocalAssetOperation.error -= this.OnCheckLocalAssetOperationError;
            
            this.Error( $"Version info cannot be found in local asset folder. \n{message}" );
        }
    }
}