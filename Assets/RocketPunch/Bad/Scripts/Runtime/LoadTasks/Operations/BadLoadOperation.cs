using System;

namespace RocketPunch.Bad
{
    public class BadLoadOperation<T> where T : BadLoadOperation<T>
    {
        public event Action<T> complete;
        
        protected void Complete()
        {
            this.complete?.Invoke( this as T );
        }
    }
}