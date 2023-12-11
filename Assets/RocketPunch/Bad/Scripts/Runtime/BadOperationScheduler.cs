using System;
using System.Collections.Generic;
using UnityEngine;

namespace RocketPunch.Bad
{
    public class BadOperationScheduler : MonoBehaviour
    {
        public static BadOperationScheduler instance
        {
            get
            {
                if( !_instance )
                {
                    var go = new GameObject( "BadTaskScheduler" );
                    _instance = go.AddComponent<BadOperationScheduler>();
                    
                    DontDestroyOnLoad( go );
                }
                return _instance;
            }
        }

        private static BadOperationScheduler _instance;
        
        private Queue<BadOperation> _pendingOperations = new();

        private BadOperation _currentOperation;
        
        public void EnqueueOperation( BadOperation operation )
        {
            _pendingOperations.Enqueue( operation );
        }

        private void Update()
        {
            if( _currentOperation != null )
            {
                return;
            }
            
            this.StartNextOperation();
        }

        private void StartNextOperation()
        {
            if( _pendingOperations.Count == 0 )
            {
                return;
            }
            
            _currentOperation = _pendingOperations.Dequeue();
            _currentOperation.complete += OnOperationCompleted;
            _currentOperation.error += OnOperationError;
            _currentOperation.Run();
        }

        private void OnOperationError( BadOperation operation, string message )
        {
            _currentOperation.complete -= OnOperationCompleted;
            _currentOperation.error -= OnOperationError;
            BadLog.Error( $"Operation error: {message}" );
            
            this.StartNextOperation();
        }

        private void OnOperationCompleted( BadOperation operation )
        {
            _currentOperation.complete -= OnOperationCompleted;
            _currentOperation.error -= OnOperationError;
            _currentOperation = null;
            this.StartNextOperation();
        }
    }
}