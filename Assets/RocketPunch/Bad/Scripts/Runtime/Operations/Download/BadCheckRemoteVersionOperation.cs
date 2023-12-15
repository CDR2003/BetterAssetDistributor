using System.IO;

namespace RocketPunch.Bad
{
    public class BadCheckRemoteVersionOperation : BadOperation<BadVersionInfo>
    {
        private BadReadFileOperation _operation;
        
        public override void Run()
        {
            var url = BadPathHelper.GetRemoteAssetPath( BadVersionInfo.Filename );
            _operation = new BadReadFileOperation( url );
            _operation.complete += this.OnOperationCompleted;
            _operation.error += this.OnOperationError;
            _operation.Run();
        }

        private void OnOperationCompleted( BadOperation operation )
        {
            _operation.complete -= this.OnOperationCompleted;
            _operation.error -= this.OnOperationError;

            var versionInfo = BadVersionInfo.ReadFromBytes( _operation.value );
            versionInfo.assetInfoFilePath = BadPathHelper.GetRemoteAssetPath( versionInfo.assetInfoFilePath );
            this.Complete( versionInfo );
        }

        private void OnOperationError( BadOperation operation, string message )
        {
            _operation.complete -= this.OnOperationCompleted;
            _operation.error -= this.OnOperationError;
            
            this.Error( $"Version info cannot be found in remote asset folder. \n{message}" );
        }
    }
}