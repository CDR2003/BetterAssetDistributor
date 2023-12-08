namespace RocketPunch.Bad
{
    public class BadAssetAsyncLoadTask : BadAsyncLoadTask
    {
        public UnityEngine.Object obj => this.asset.loadedInfo.obj;
        
        public readonly BadAssetInfo asset;
        
        private BadAssetLoadOperation _operation;

        public BadAssetAsyncLoadTask( BadAssetInfo asset )
        {
            this.asset = asset;
        }
        
        public override void Run()
        {
            _operation = this.asset.LoadAsync();
            if( _operation == null )
            {
                this.Complete();
                return;
            }
            
            _operation.complete += this.OnOperationCompleted;
        }

        private void OnOperationCompleted( BadAssetLoadOperation operation )
        {
            _operation.complete -= this.OnOperationCompleted;
            this.Complete();
        }
    }
}