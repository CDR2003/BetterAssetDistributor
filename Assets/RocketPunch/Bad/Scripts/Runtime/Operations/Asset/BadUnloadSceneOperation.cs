using UnityEngine;
using UnityEngine.SceneManagement;

namespace RocketPunch.Bad
{
    public class BadUnloadSceneOperation : BadUnloadAssetOperation
    {
        public BadUnloadSceneOperation(BadAssetInfo assetInfo) : base(assetInfo)
        {
            
        }

        public override void Run()
        {
            var scenePaths = _assetInfo.bundle.bundle.GetAllScenePaths();
            var targetScene = SceneManager.GetSceneByName(scenePaths[0]);
            if (targetScene.isLoaded)
            {
                var handler = SceneManager.UnloadSceneAsync(scenePaths[0]);
                handler.completed += RunBase;
            }
            else
            {
                ClearDependcy();
            }
        }

        private void RunBase(AsyncOperation asyncOperation)
        {
            asyncOperation.completed -= RunBase;
            ClearDependcy();
        }

        private void ClearDependcy()
        {
            BadLog.Info( $"[ASYNC] Unloaded scene '{_assetInfo.name}' ({_assetInfo.guid}) from bundle '{_assetInfo.bundle.name}'" );
            _postOperation = new BadSequenceOperation();
            _postOperation.Add( new BadUnloadBundleOperation( _assetInfo.bundle ) );
            foreach( var dependency in _assetInfo.dependencies )
            {
                var operation = dependency.UnloadAsync();
                _postOperation.Add( operation );
            }
            
            _postOperation.complete += OnSequenceCompleted;
            _postOperation.error += OnSequenceError;
            _postOperation.Run();
        }
        
        public override string ToString()
        {
            return $"UnloadScene: {_assetInfo.name} ({_assetInfo.guid}) @ '{_assetInfo.bundle.name}'";
        }
    }
}