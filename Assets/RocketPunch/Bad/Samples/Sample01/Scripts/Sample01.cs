using System.Collections.Generic;
using UnityEngine;

namespace RocketPunch.Bad.Samples
{
    public class Sample01 : MonoBehaviour
    {
        public string infoFileName;

        private GameObject _cubePrefab;

        private BadAssetAsyncLoadTask _loadTask;

        private Stack<GameObject> _cubes = new();
        
        public void Start()
        {
            BadAssetLibrary.Load( $"AssetBundles/{this.infoFileName}.bad" );
        }

        private void OnLoadTaskCompleted( BadAsyncLoadTask obj )
        {
            _loadTask.complete -= this.OnLoadTaskCompleted;
            
            _cubePrefab = _loadTask.obj as GameObject;
            
            var cube = Instantiate( _cubePrefab );
            _cubes.Push( cube );

            Debug.Assert( cube.GetComponent<MeshRenderer>().sharedMaterial.mainTexture );
        }

        private void Update()
        {
            if( Input.GetMouseButtonDown( 0 ) )
            {
                _loadTask = BadLoader.LoadAsync<GameObject>( "Cube" );
                _loadTask.complete += this.OnLoadTaskCompleted;
            }
            
            if( Input.GetMouseButtonDown( 1 ) )
            {
                if( _cubes.Count > 0 )
                {
                    var cube = _cubes.Pop();
                    Destroy( cube );
                    BadLoader.Unload( _cubePrefab );
                }
            }
        }
    }
}