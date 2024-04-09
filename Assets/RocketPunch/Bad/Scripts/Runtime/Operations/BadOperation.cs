using System;
using Object = UnityEngine.Object;

namespace RocketPunch.Bad
{
    public abstract class BadOperation
    {
        public delegate void CompleteDelegate( BadOperation operation );
        
        public delegate void ErrorDelegate( BadOperation operation, string message );
        
        public event CompleteDelegate complete;

        public event ErrorDelegate error;

        public bool isCompleted { get; protected set; }
        
        public abstract void Run();

        protected virtual void Cleanup()
        {
        }
        
        protected void Complete()
        {
            this.complete?.Invoke( this );
            this.Cleanup();
        }
        
        protected void Error( string message )
        {
            this.error?.Invoke( this, message );
            this.Cleanup();
        }
    }
    
    public abstract class BadOperation<T> : BadOperation
    {
        public new delegate void CompleteDelegate( BadOperation<T> operation );
        
        public new event CompleteDelegate complete;

        public T value { get; private set; }

        protected void Complete( T value )
        {
            this.value = value;
            complete?.Invoke( this );
            Cleanup();
            isCompleted = true;
            base.Complete();
        }
    }
}