using System;

namespace RocketPunch.Bad
{
    public abstract class BadAsyncLoadTask : BadLoadTask
    {
        public event Action<BadAsyncLoadTask> complete;

        protected void Complete()
        {
            this.complete?.Invoke( this );
        }
    }
}