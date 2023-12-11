using System;

namespace RocketPunch.Bad.Operations
{
    public class BadUnloadAssetOperation : BadOperation
    {
        private readonly BadAssetInfo _assetInfo;

        private BadSequenceOperation _postOperation;
        
        public BadUnloadAssetOperation( BadAssetInfo assetInfo )
        {
            _assetInfo = assetInfo;
        }
        
        public override void Run()
        {
            if( _assetInfo.hasLoaded == false )
            {
                throw new Exception( $"Asset '{_assetInfo.name}' ({_assetInfo.guid}) is not loaded" );
            }
            
            _assetInfo.loadedInfo.referenceCount--;
            if( _assetInfo.loadedInfo.referenceCount > 0 )
            {
                this.Complete();
                return;
            }
            
            BadLoadedAssetLibrary.Remove( _assetInfo.loadedInfo );
            _assetInfo.loadedInfo = null;
            
            BadLog.Info( $"[ASYNC] Unloaded asset '{_assetInfo.name}' ({_assetInfo.guid}) from bundle '{_assetInfo.bundle.name}'" );
            
            _postOperation = new BadSequenceOperation();
            _postOperation.Add( new BadUnloadBundleOperation( _assetInfo.bundle ) );
            foreach( var dependency in _assetInfo.dependencies )
            {
                var operation = dependency.UnloadAsync();
                _postOperation.Add( operation );
            }
            
            _postOperation.complete += this.OnSequenceCompleted;
            _postOperation.error += this.OnSequenceError;
            _postOperation.Run();
        }

        public override string ToString()
        {
            return $"UnloadAsset: {_assetInfo.name} ({_assetInfo.guid}) @ '{_assetInfo.bundle.name}'";
        }

        private void OnSequenceCompleted( BadOperation operation )
        {
            _postOperation.complete -= this.OnSequenceCompleted;
            _postOperation.error -= this.OnSequenceError;
            this.Complete();
        }
        
        private void OnSequenceError( BadOperation operation, string message )
        {
            _postOperation.complete -= this.OnSequenceCompleted;
            _postOperation.error -= this.OnSequenceError;
            this.Error( $"Inner operation error: {message}" );
        }
    }
}