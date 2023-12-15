using System.Collections.Generic;

namespace RocketPunch.Bad
{
    public class BadSequenceOperation : BadOperation
    {
        private readonly List<BadOperation> _operations = new();
        
        private int _currentIndex;

        public BadSequenceOperation( params BadOperation[] operations )
        {
            _operations.AddRange( operations );
        }
        
        public void Add( BadOperation operation )
        {
            _operations.Add( operation );
        }
        
        public override void Run()
        {
            _currentIndex = -1;
            this.StartNextOperation();
        }

        public override string ToString()
        {
            var strOperations = string.Join( ", ", _operations );
            return $"Sequence: [{strOperations}]";
        }

        protected override void Cleanup()
        {
            this.CleanupCurrentOperation();
        }

        private void StartNextOperation()
        {
            _currentIndex++;
            if( _currentIndex >= _operations.Count )
            {
                this.Complete();
                return;
            }

            var operation = _operations[_currentIndex];
            operation.complete += this.OnOperationCompleted;
            operation.error += this.OnOperationError;
            operation.Run();
        }

        private void CleanupCurrentOperation()
        {
            if( _currentIndex < 0 || _currentIndex >= _operations.Count )
            {
                return;
            }
            
            var operation = _operations[_currentIndex];
            operation.complete -= this.OnOperationCompleted;
            operation.error -= this.OnOperationError;
        }

        private void OnOperationError( BadOperation operation, string message )
        {
            this.Error( message );
        }

        private void OnOperationCompleted( BadOperation operation )
        {
            this.CleanupCurrentOperation();
            this.StartNextOperation();
        }
    }
}