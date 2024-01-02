using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RocketPunch.Bad
{
    public class BadUpdateWindow : MonoBehaviour
    {
        public BadErrorDialog dlgError;
        
        public BadUpdateConfirmDialog dlgUpdateConfirm;
        
        public BadDownloadDialog dlgDownload;

        public string startSceneName;

        private BadDownloader _downloader;
        
        private void Start()
        {
            var updateManager = BadUpdateManager.instance;
            updateManager.Initialize();
            updateManager.versionCheck += this.OnVersionCheck;
            updateManager.error += this.OnError;
            updateManager.CheckVersion();
            
            this.dlgUpdateConfirm.confirm += this.OnUpdateConfirm;
            this.dlgUpdateConfirm.cancel += this.OnUpdateCancel;
        }
        
        private void OnDestroy()
        {
            var updateManager = BadUpdateManager.instance;
            updateManager.versionCheck -= this.OnVersionCheck;
            updateManager.error -= this.OnError;
            
            this.dlgUpdateConfirm.confirm -= this.OnUpdateConfirm;
            this.dlgUpdateConfirm.cancel -= this.OnUpdateCancel;
        }

        private void OnError( string message )
        {
            this.dlgError.gameObject.SetActive( true );
        }

        private void OnVersionCheck( BadVersionCheckResult result )
        {
            if( result.needsUpdate == false )
            {
                this.EnterGame();
                return;
            }
            
            this.ShowUpdateConfirmDialog( result );
        }

        private void ShowUpdateConfirmDialog( BadVersionCheckResult result )
        {
            this.dlgUpdateConfirm.SetResult( result );
            this.dlgUpdateConfirm.gameObject.SetActive( true );
        }
        
        private void OnUpdateConfirm()
        {
            this.ShowDownloadDialog();
        }
        
        private void OnUpdateCancel()
        {
            Application.Quit();
        }
        
        private void ShowDownloadDialog()
        {
            _downloader = BadUpdateManager.instance.StartDownload();
            _downloader.complete += this.OnDownloadComplete;
            _downloader.error += this.OnDownloadError;
            
            this.dlgDownload.downloader = _downloader;
            this.dlgDownload.gameObject.SetActive( true );
        }

        private void OnDownloadComplete()
        {
            _downloader.complete -= this.OnDownloadComplete;
            
            this.dlgDownload.gameObject.SetActive( false );
            this.EnterGame();
        }
        
        private void OnDownloadError( string message )
        {
            _downloader.error -= this.OnDownloadError;
            
            this.dlgDownload.gameObject.SetActive( false );
            
            this.dlgError.message = message;
            this.dlgError.gameObject.SetActive( true );
        }

        private void EnterGame()
        {
            BadLoader.initializeComplete += this.OnInitializeComplete;
            BadLoader.Initialize();
        }

        private void OnInitializeComplete()
        {
            BadLoader.initializeComplete -= this.OnInitializeComplete;

            SceneManager.LoadScene( this.startSceneName );
        }
    }
}