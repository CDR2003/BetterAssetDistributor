namespace RocketPunch.Bad
{
    public class BadUpdateManager
    {
        private BadVersionInfo _localVersionInfo;
        
        private BadVersionInfo _remoteVersionInfo;
        
        public BadCheckVersionOperation CheckVersion()
        {
            var operation = new BadCheckVersionOperation();
            operation.complete += this.OnCheckVersionCompleted;
            operation.error += this.OnCheckVersionError;
            BadOperationScheduler.instance.EnqueueOperation( operation );
            return operation;
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
            
            var checkVersionOperation = operation as BadCheckVersionOperation;
            _localVersionInfo = checkVersionOperation.localVersion;
            _remoteVersionInfo = checkVersionOperation.remoteVersion;
        }
    }
}