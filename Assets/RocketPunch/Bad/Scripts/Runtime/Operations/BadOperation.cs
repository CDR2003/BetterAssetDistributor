using System;

namespace RocketPunch.Bad
{
    public abstract class BadOperation
    {
        public delegate void CompleteDelegate( BadOperation operation );
        
        public delegate void ErrorDelegate( BadOperation operation, string message );
        
        public event CompleteDelegate complete;

        public event ErrorDelegate error;
        
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
        public T value { get; private set; }

        protected void Complete( T value )
        {
            this.value = value;
            base.Complete();
        }
    }
}