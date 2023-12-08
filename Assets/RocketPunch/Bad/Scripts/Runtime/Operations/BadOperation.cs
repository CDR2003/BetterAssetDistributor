using System;

namespace RocketPunch.Bad
{
    public abstract class BadOperation
    {
        public string errorMessage { get; private set; }

        public event Action<BadOperation> complete;

        public event Action<BadOperation> error;
        
        public abstract void Run();
        
        protected void Complete()
        {
            this.complete?.Invoke( this );
        }
        
        protected void Error( string message )
        {
            this.errorMessage = message;
            this.error?.Invoke( this );
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