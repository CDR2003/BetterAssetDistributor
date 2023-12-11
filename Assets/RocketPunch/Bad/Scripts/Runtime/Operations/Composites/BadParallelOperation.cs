using System.Collections.Generic;

namespace RocketPunch.Bad.Operations
{
    public class BadParallelOperation : BadOperation
    {
        private readonly List<BadOperation> _operations = new();

        private readonly List<BadOperation> _runningOperations = new();
        
        public BadParallelOperation( params BadOperation[] operations )
        {
            _operations.AddRange( operations );
        }
        
        public void Add( BadOperation operation )
        {
            _operations.Add( operation );
        }

        public override void Run()
        {
            foreach( var operation in _operations )
            {
                operation.complete += this.OnOperationCompleted;
                operation.error += this.OnOperationError;
                operation.Run();
            }
        }

        public override string ToString()
        {
            var strOperations = string.Join( ", ", _operations );
            return $"Parallel: ({strOperations})";
        }

        protected override void Cleanup()
        {
            foreach( var operation in _operations )
            {
                operation.complete -= this.OnOperationCompleted;
                operation.error -= this.OnOperationError;
            }
        }

        private void OnOperationError( BadOperation operation, string message )
        {
            this.Error( $"Inner operation error: {message}" );
        }

        private void OnOperationCompleted( BadOperation operation )
        {
            _runningOperations.Add( operation );
            
            if( _runningOperations.Count == _operations.Count )
            {
                this.Complete();
            }
        }
    }
}