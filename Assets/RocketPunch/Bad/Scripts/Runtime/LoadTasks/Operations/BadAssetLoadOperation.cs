using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RocketPunch.Bad
{
    public class BadAssetLoadOperation : BadLoadOperation<BadAssetLoadOperation>
    {
        public Object asset => _asset.loadedInfo.obj;
        
        private readonly BadAssetInfo _asset;

        private readonly AssetBundleRequest _request;
        
        public BadAssetLoadOperation( BadAssetInfo asset, AssetBundleRequest request )
        {
            _asset = asset;
            _request = request;
            _request.completed += this.OnCompleted;
        }

        private void OnCompleted( AsyncOperation obj )
        {
            _request.completed -= this.OnCompleted;

            if( _request.asset == null )
            {
                throw new Exception( $"Failed to load asset '{_asset.name}' from bundle '{_asset.bundle.name}'" );
            }

            _asset.loadedInfo = new BadLoadedAssetInfo( _asset, _request.asset );
            _asset.loadedInfo.referenceCount++;
            
            BadLoadedAssetLibrary.Add( _asset.loadedInfo );
            
            BadLog.Info( $"[ASYNC] Loaded asset '{_asset.name}' ({_asset.guid}) from bundle '{_asset.bundle.name}'" );
            
            this.Complete();
        }
    }
}