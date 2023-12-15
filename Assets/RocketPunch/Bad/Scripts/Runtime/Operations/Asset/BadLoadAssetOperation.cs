using UnityEngine;

namespace RocketPunch.Bad
{
    public class BadLoadAssetOperation : BadOperation<Object>
    {
        private readonly BadAssetInfo _assetInfo;

        private AssetBundleRequest _request;

        private BadSequenceOperation _requiredOperation;
        
        public BadLoadAssetOperation( BadAssetInfo assetInfo )
        {
            _assetInfo = assetInfo;
            this.InitializeRequiredOperation();
        }

        private void InitializeRequiredOperation()
        {
            _requiredOperation = new BadSequenceOperation();
            if( _assetInfo.dependencies.Count > 0 )
            {
                var dependencyOperation = new BadSequenceOperation();
                foreach( var dependency in _assetInfo.dependencies )
                {
                    var operation = dependency.LoadAsync();
                    dependencyOperation.Add( operation );
                }
                _requiredOperation.Add( dependencyOperation );
            }

            var bundleOperation = _assetInfo.bundle.LoadAsync();
            _requiredOperation.Add( bundleOperation );
        }

        public override void Run()
        {
            if( _assetInfo.hasLoaded )
            {
                foreach( var dependency in _assetInfo.dependencies )
                {
                    // Invoke Load() just to increase reference count
                    dependency.Load();
                }
                _assetInfo.loadedInfo.referenceCount++;
                
                this.Complete( _assetInfo.loadedInfo.obj );
                return;
            }
            
            _requiredOperation.complete += this.OnRequiredOperationCompleted;
            _requiredOperation.error += this.OnRequiredOperationError;
            _requiredOperation.Run();
        }

        public override string ToString()
        {
            return $"LoadAsset: {_assetInfo.name} ({_assetInfo.guid}) @ '{_assetInfo.bundle.name}'";
        }

        private void OnRequiredOperationCompleted( BadOperation operation )
        {
            _requiredOperation.complete -= this.OnRequiredOperationCompleted;
            _requiredOperation.error -= this.OnRequiredOperationError;

            var bundle = _assetInfo.bundle.bundle;
            _request = bundle.LoadAssetAsync( _assetInfo.guid );
            _request.completed += this.OnRequestCompleted;
        }
        
        private void OnRequiredOperationError( BadOperation operation, string message )
        {
            _requiredOperation.complete -= this.OnRequiredOperationCompleted;
            _requiredOperation.error -= this.OnRequiredOperationError;
            this.Error( $"Failed to load asset '{_assetInfo.name}' ({_assetInfo.guid}) @ '{_assetInfo.bundle.name}'. \n{message}" );
        }

        private void OnRequestCompleted( AsyncOperation obj )
        {
            _request.completed -= this.OnRequestCompleted;
            
            if( _request.isDone == false )
            {
                this.Error( $"Failed to load asset '{_assetInfo.name}' ({_assetInfo.guid}) @ '{_assetInfo.bundle.name}'" );
                return;
            }
            
            _assetInfo.loadedInfo = new BadLoadedAssetInfo( _assetInfo, _request.asset );
            _assetInfo.loadedInfo.referenceCount++;
            BadLoadedAssetLibrary.Add( _assetInfo.loadedInfo );
            
            BadLog.Info( $"[ASYNC] Loaded asset '{_assetInfo.name}' ({_assetInfo.guid}) from bundle '{_assetInfo.bundle.name}'" );
            
            this.Complete( _assetInfo.loadedInfo.obj );
        }
    }
}