namespace RocketPunch.Bad
{
    public class BadReadAssetInfoFileOperation : BadOperation<BadAssetInfoFile>
    {
        private string _path;

        private BadReadFileOperation _operation;
        
        public BadReadAssetInfoFileOperation( string path )
        {
            _path = path;
        }
        
        public override void Run()
        {
            _operation = new BadReadFileOperation( _path );
            _operation.complete += this.OnOperationCompleted;
            _operation.error += this.OnOperationError;
            _operation.Run();
        }

        private void OnOperationError( BadOperation operation, string message )
        {
            _operation.complete -= this.OnOperationCompleted;
            _operation.error -= this.OnOperationError;
            
            this.Error( $"Failed to read asset info file '{_path}'. \n{message}" );
        }

        private void OnOperationCompleted( BadOperation operation )
        {
            _operation.complete -= this.OnOperationCompleted;
            _operation.error -= this.OnOperationError;

            var assetInfoFile = BadAssetInfoFile.ReadFromFile( _operation.value );
            this.Complete( assetInfoFile );
        }
    }
}