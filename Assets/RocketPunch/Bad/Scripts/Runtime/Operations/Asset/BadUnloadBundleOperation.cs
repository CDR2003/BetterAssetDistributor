using System;
using UnityEngine;

namespace RocketPunch.Bad.Operations
{
    public class BadUnloadBundleOperation : BadOperation
    {
        private readonly BadBundleInfo _bundleInfo;

        private AsyncOperation _request;
        
        public BadUnloadBundleOperation( BadBundleInfo bundleInfo )
        {
            _bundleInfo = bundleInfo;
        }
        
        public override void Run()
        {
            if( _bundleInfo.hasLoaded == false )
            {
                throw new Exception( $"Bundle '{_bundleInfo.name}' is not loaded" );
            }

            if( _bundleInfo.hasLoadedAssets )
            {
                this.Complete();
                return;
            }
            
            _bundleInfo.state = BadBundleState.Unloading;
            
            _request = _bundleInfo.bundle.UnloadAsync( true );
            _request.completed += this.OnRequestCompleted;
        }

        public override string ToString()
        {
            return $"UnloadBundle: {_bundleInfo.name}";
        }

        private void OnRequestCompleted( AsyncOperation obj )
        {
            _request.completed -= this.OnRequestCompleted;
            
            _bundleInfo.bundle = null;
            _bundleInfo.state = BadBundleState.Downloaded;
            
            BadLog.Info( $"[ASYNC] Unloaded bundle '{_bundleInfo.name}'" );
            
            this.Complete();
        }
    }
}