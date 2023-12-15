using System.Collections.Generic;
using UnityEngine;

namespace RocketPunch.Bad.Samples
{
    public class Sample01 : MonoBehaviour
    {
        public string infoFileName;

        private GameObject _cubePrefab;

        private BadLoadAssetOperation _loadOperation;

        private Stack<GameObject> _cubes = new();
        
        public void Start()
        {
            BadSettings.Load();
            
            var operation = new BadCheckVersionOperation();
            operation.complete += this.OnCheckVersionCompleted;
            operation.error += this.OnCheckVersionError;
            operation.Run();
        }

        private void OnCheckVersionError( BadOperation operation, string message )
        {
            operation.complete -= this.OnCheckVersionCompleted;
            operation.error -= this.OnCheckVersionError;
            
            Debug.LogError( message );
        }

        private void OnCheckVersionCompleted( BadOperation operation )
        {
            operation.complete -= this.OnCheckVersionCompleted;
            operation.error -= this.OnCheckVersionError;
            
            var checkVersionOperation = operation as BadCheckVersionOperation;
            Debug.Log( $"Local version: {checkVersionOperation.localVersion.version}" );
            Debug.Log( $"Remote version: {checkVersionOperation.remoteVersion.version}" );
        }

        private void OnLoadOperationCompleted( BadOperation operation )
        {
            _loadOperation.complete -= this.OnLoadOperationCompleted;
            
            _cubePrefab = _loadOperation.value as GameObject;
            
            var cube = Instantiate( _cubePrefab );
            _cubes.Push( cube );

            Debug.Assert( cube.GetComponent<MeshRenderer>().sharedMaterial.mainTexture );
        }

        private void Update()
        {
            if( Input.GetMouseButtonDown( 0 ) )
            {
                _loadOperation = BadLoader.LoadAsync<GameObject>( "Cube" );
                _loadOperation.complete += this.OnLoadOperationCompleted;
            }
            
            if( Input.GetMouseButtonDown( 1 ) )
            {
                if( _cubes.Count > 0 )
                {
                    var cube = _cubes.Pop();
                    Destroy( cube );
                    BadLoader.UnloadAsync( _cubePrefab );
                }
            }
        }
    }
}