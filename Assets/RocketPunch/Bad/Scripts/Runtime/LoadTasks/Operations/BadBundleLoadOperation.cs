using System;
using UnityEngine;

namespace RocketPunch.Bad
{
    public class BadBundleLoadOperation : BadLoadOperation<BadBundleLoadOperation>
    {
        private readonly BadBundleInfo _bundle;
        
        private readonly AssetBundleCreateRequest _request;
        
        public BadBundleLoadOperation( BadBundleInfo bundle, AssetBundleCreateRequest request )
        {
            _bundle = bundle;
            _request = request;
            _request.completed += this.OnCompleted;
        }

        private void OnCompleted( AsyncOperation obj )
        {
            _request.completed -= this.OnCompleted;

            _bundle.bundle = _request.assetBundle;
            if( _bundle.bundle == null )
            {
                throw new Exception( $"Failed to load bundle '{_bundle.name}'" );
            }

            _bundle.state = BadBundleState.Loaded;
            
            BadLog.Info( $"[ASYNC] Loaded bundle '{_bundle.name}'" );

            this.Complete();
        }
    }
}