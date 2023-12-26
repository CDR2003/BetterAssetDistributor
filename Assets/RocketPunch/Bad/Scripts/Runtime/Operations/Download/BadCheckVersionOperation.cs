namespace RocketPunch.Bad
{
    public class BadCheckVersionOperation : BadOperation
    {
        public BadVersionInfo localVersion { get; private set; }
        
        public BadVersionInfo remoteVersion { get; private set; }

        public BadVersionInfoSource localVersionSource => _localVersionOperation.source;

        private BadCheckLocalVersionOperation _localVersionOperation;
        
        private BadCheckRemoteVersionOperation _remoteVersionOperation;

        private BadParallelOperation _parallelOperation;

        public override void Run()
        {
            _localVersionOperation = new BadCheckLocalVersionOperation();
            _remoteVersionOperation = new BadCheckRemoteVersionOperation();
            
            _parallelOperation = new BadParallelOperation( _localVersionOperation, _remoteVersionOperation );
            _parallelOperation.complete += this.OnParallelOperationCompleted;
            _parallelOperation.error += this.OnParallelOperationError;
            _parallelOperation.Run();
        }

        private void OnParallelOperationCompleted( BadOperation operation )
        {
            _parallelOperation.complete -= this.OnParallelOperationCompleted;
            _parallelOperation.error -= this.OnParallelOperationError;
            
            this.localVersion = _localVersionOperation.value;
            this.remoteVersion = _remoteVersionOperation.value;
            this.Complete();
        }

        private void OnParallelOperationError( BadOperation operation, string message )
        {
            _parallelOperation.complete -= this.OnParallelOperationCompleted;
            _parallelOperation.error -= this.OnParallelOperationError;
            
            this.Error( $"Version check failed. \n{message}" );
        }
    }
}