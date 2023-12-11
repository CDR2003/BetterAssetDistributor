using System.Collections.Generic;
using RocketPunch.Bad.Operations;
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
            BadAssetLibrary.Load( $"AssetBundles/{this.infoFileName}.bad" );
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