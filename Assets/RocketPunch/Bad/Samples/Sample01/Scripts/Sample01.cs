using System;
using UnityEngine;

namespace RocketPunch.Bad.Samples
{
    public class Sample01 : MonoBehaviour
    {
        public string infoFileName;

        private GameObject _cubePrefab;

        private GameObject _spherePrefab;
        
        public void Start()
        {
            BadAssetLibrary.Load( $"AssetBundles/{this.infoFileName}.bad" );
            
            _spherePrefab = BadLoader.Load<GameObject>( "Sphere" );
            Instantiate( _spherePrefab );

            _cubePrefab = BadLoader.Load<GameObject>( "Cube" );
            Instantiate( _cubePrefab );
        }

        private void OnDestroy()
        {
            BadLoader.Unload( _cubePrefab );
            _cubePrefab = null;
            
            BadLoader.Unload( _spherePrefab );
            _spherePrefab = null;
        }
    }
}