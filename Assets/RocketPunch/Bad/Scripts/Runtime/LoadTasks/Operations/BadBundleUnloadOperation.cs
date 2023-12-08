using UnityEngine;

namespace RocketPunch.Bad
{
    public class BadBundleUnloadOperation : BadLoadOperation<BadBundleUnloadOperation>
    {
        private readonly BadBundleInfo _bundle;

        private readonly AsyncOperation _request;
        
        public BadBundleUnloadOperation( BadBundleInfo bundle, AsyncOperation request )
        {
            _bundle = bundle;
            _request = request;
            _request.completed += this.OnCompleted;
        }
        
        private void OnCompleted( AsyncOperation obj )
        {
            _request.completed -= this.OnCompleted;

            _bundle.bundle = null;
            _bundle.state = BadBundleState.Downloaded;
            
            BadLog.Info( $"[ASYNC] Unloaded bundle '{_bundle.name}'" );

            this.Complete();
        }
    }
}