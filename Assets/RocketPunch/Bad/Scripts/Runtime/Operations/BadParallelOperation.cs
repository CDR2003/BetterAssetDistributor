using System.Collections.Generic;

namespace RocketPunch.Bad
{
    public class BadParallelOperation : BadOperation
    {
        private readonly List<BadOperation> _operations = new();

        private readonly List<BadOperation> _runningOperations = new();
        
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

        private void OnOperationError( BadOperation operation )
        {
            
        }

        private void OnOperationCompleted( BadOperation operation )
        {
            _runningOperations.Add( operation );
        }
    }
}