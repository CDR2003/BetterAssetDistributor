using UnityEngine;
using UnityEngine.SceneManagement;

namespace RocketPunch.Bad
{
    public class BadLoadSceneOperation : BadOperation
    {
        private readonly BadAssetInfo _assetInfo;

        private AsyncOperation _sceneLoader;

        private BadSequenceOperation _requiredOperation;

        private LoadSceneMode _loadSceneMode;
        
        public BadLoadSceneOperation(BadAssetInfo assetInfo, LoadSceneMode loadSceneMode)
        {
            _assetInfo = assetInfo;
            _loadSceneMode = loadSceneMode;
            InitializeRequiredOperation();
        }

        public BadUnloadSceneOperation UnloadAsync()
        {
            return _assetInfo.UnloadSceneAsync();
        }

        private void InitializeRequiredOperation()
        {
            _requiredOperation = new BadSequenceOperation();
            if( _assetInfo.dependencies.Count > 0 )
            {
                var dependencyOperation = new BadSequenceOperation();
                foreach( var dependency in _assetInfo.dependencies )
                {
                    var operation = dependency.LoadAsync<UnityEngine.Object>();
                    dependencyOperation.Add( operation );
                }
                _requiredOperation.Add( dependencyOperation );
            }

            var bundleOperation = _assetInfo.bundle.LoadAsync();
            _requiredOperation.Add( bundleOperation );
        }

        public override void Run()
        {
            // if( _assetInfo.hasLoaded )
            // {
            //     foreach( var dependency in _assetInfo.dependencies )
            //     {
            //         // Invoke Load() just to increase reference count
            //         dependency.Load();
            //     }
            //     _assetInfo.loadedInfo.referenceCount++;
            //     
            //     this.Complete();
            //     return;
            // }
            
            _requiredOperation.complete += this.OnRequiredOperationCompleted;
            _requiredOperation.error += this.OnRequiredOperationError;
            _requiredOperation.Run();
        }

        public override string ToString()
        {
            return $"LoadScene: {_assetInfo} ({_assetInfo.guid}) @ '{_assetInfo.bundle.name}'";
        }
        
        private void OnRequiredOperationCompleted( BadOperation operation )
        {
            _requiredOperation.complete -= this.OnRequiredOperationCompleted;
            _requiredOperation.error -= this.OnRequiredOperationError;

            // var bundle = _assetInfo.bundle.bundle;
            // _request = bundle.LoadAllAssetsAsync();
            // _request.completed += this.OnRequestCompleted;
            LoadScene();
        }
        
        private void OnRequiredOperationError( BadOperation operation, string message )
        {
            _requiredOperation.complete -= this.OnRequiredOperationCompleted;
            _requiredOperation.error -= this.OnRequiredOperationError;
            this.Error( $"Failed to load asset '{_assetInfo.name}' ({_assetInfo.guid}) @ '{_assetInfo.bundle.name}'. \n{message}" );
        }

        private void LoadScene()
        {
            // _request.completed -= this.OnRequestCompleted;
            //
            // if( _request.isDone == false )
            // {
            //     this.Error( $"Failed to load scene '{_assetInfo.name}' ({_assetInfo.guid}) @ '{_assetInfo.bundle.name}'" );
            //     return;
            // }

            var scenePaths = _assetInfo.bundle.bundle.GetAllScenePaths();
            if (scenePaths.Length == 0)
            {
                this.Error( $"Failed to load scene '{_assetInfo.name}' ({_assetInfo.guid}) @ '{_assetInfo.bundle.name}'" );
                return;
            }
            
            // _assetInfo.loadedInfo = new BadLoadedAssetInfo( _assetInfo, _request.asset );
            // _assetInfo.loadedInfo.referenceCount++;
            // BadLoadedAssetLibrary.Add( _assetInfo.loadedInfo );

            _sceneLoader = SceneManager.LoadSceneAsync(scenePaths[0], _loadSceneMode);
            _sceneLoader.completed += OnSceneLoadCompleted;
        }

        private void OnSceneLoadCompleted(AsyncOperation operation)
        {
            _sceneLoader.completed -= OnSceneLoadCompleted;

            if (operation.isDone == false)
            {
                this.Error( $"Failed to install scene '{_assetInfo.name}' ({_assetInfo.guid}) @ '{_assetInfo.bundle.name}'" );
                return;
            }
            
            
            BadLog.Info( $"[ASYNC] Loaded scene '{_assetInfo.name}' ({_assetInfo.guid}) from bundle '{_assetInfo.bundle.name}'" );
            
            this.Complete();
        }
    }
}