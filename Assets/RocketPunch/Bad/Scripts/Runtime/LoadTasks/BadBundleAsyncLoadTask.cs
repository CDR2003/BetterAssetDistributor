namespace RocketPunch.Bad
{
    public class BadBundleAsyncLoadTask : BadAsyncLoadTask
    {
        public readonly BadBundleInfo bundle;
        
        private BadBundleLoadOperation _operation;
        
        public BadBundleAsyncLoadTask( BadBundleInfo bundle )
        {
            this.bundle = bundle;
        }
        
        public override void Run()
        {
            _operation = this.bundle.LoadAsync();
            if( _operation == null )
            {
                this.Complete();
                return;
            }
            
            _operation.complete += this.OnOperationCompleted;
        }

        private void OnOperationCompleted( BadBundleLoadOperation operation )
        {
            _operation.complete -= this.OnOperationCompleted;
            this.Complete();
        }
    }
}