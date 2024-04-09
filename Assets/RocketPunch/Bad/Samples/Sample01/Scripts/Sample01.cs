using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RocketPunch.Bad.Samples
{
    public class Sample01 : MonoBehaviour
    {
        public string infoFileName;

        private GameObject _cubePrefab;

        private BadLoadAssetOperation<GameObject> _loadOperation;

        private Stack<GameObject> _cubes = new();

        public void Start()
        {
            BadSettings.Load();
            BadPathHelper.MakeDownloadFolder();

            var updateManager = BadUpdateManager.instance;
            updateManager.versionCheck += this.OnVersionCheck;
            updateManager.error += this.OnError;
            updateManager.CheckVersion();
            
            DontDestroyOnLoad(this);
        }

        private void OnError( string message )
        {
            var updateManager = BadUpdateManager.instance;
            updateManager.versionCheck -= this.OnVersionCheck;
            updateManager.error -= this.OnError;
            
            Debug.LogError( message );
        }

        private void OnVersionCheck( BadVersionCheckResult result )
        {
            var updateManager = BadUpdateManager.instance;
            updateManager.versionCheck -= this.OnVersionCheck;
            updateManager.error -= this.OnError;
            
            Debug.Log( $"Version check completed. \nLocal version: {result.localVersion} \nRemote version: {result.remoteVersion}" );

            if( result.needsUpdate == false )
            {
                this.LoadAssetLibrary();
                return;
            }
            
            Debug.Log( $"Download size: {result.downloadedSize}B / {result.totalDownloadSize}B" );

            var downloader = updateManager.StartDownload();
            downloader.complete += this.OnDownloadComplete;
            downloader.error += this.OnDownloadError;
        }

        private void OnDownloadError( string message )
        {
            var downloader = BadUpdateManager.instance.StartDownload();
            downloader.complete -= this.OnDownloadComplete;
            downloader.error -= this.OnDownloadError;
            
            Debug.LogError( message );
        }

        private void OnDownloadComplete()
        {
            var downloader = BadUpdateManager.instance.StartDownload();
            downloader.complete -= this.OnDownloadComplete;
            downloader.error -= this.OnDownloadError;
            
            Debug.Log( $"Download completed! Please open folder {Application.persistentDataPath} to check out." );
            
            this.LoadAssetLibrary();
        }

        private void LoadAssetLibrary()
        {
            BadLoader.initializeComplete += this.OnInitializeComplete;
            BadLoader.Initialize();
        }

        private void OnInitializeComplete()
        {
            BadLoader.initializeComplete -= this.OnInitializeComplete;
            Debug.Log( "BadLoader initialized.");
        }

        private void OnLoadOperationCompleted( BadOperation operation )
        {
            _loadOperation.complete -= this.OnLoadOperationCompleted;
            
            _cubePrefab = _loadOperation.value as GameObject;
            
            var cube = Instantiate( _cubePrefab );
            _cubes.Push( cube );

            Debug.Assert( cube.GetComponent<MeshRenderer>().sharedMaterial.mainTexture );
        }

        private bool newScene = false;
        private BadLoadSceneOperation sceneLoader;
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

            if (Input.GetMouseButton(2))
            {
                // SceneManager.
                if (!newScene)
                {
                    sceneLoader=BadLoader.LoadSceneAsync("dba50ef0c19a4c941824b28cb29fc22b");
                    newScene = true;
                }
                else
                {
                    SceneManager.LoadScene("Sample01");
                    BadLoader.UnloadSceneAsync(sceneLoader);
                    
                    newScene = false;
                }
            }
        }
    }
}