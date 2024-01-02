using System;
using UnityEngine;
using UnityEngine.UI;

namespace RocketPunch.Bad
{
    public class BadUpdateConfirmDialog : MonoBehaviour
    {
        public Text txtMessage;
        
        public Button btnConfirm;
        
        public Button btnCancel;

        [TextArea]
        public string message;

        public event Action confirm;
        
        public event Action cancel;

        public void SetResult( BadVersionCheckResult result )
        {
            var downloadSize = result.totalDownloadSize - result.downloadedSize;
            this.txtMessage.text = string.Format( this.message, result.localVersion, result.remoteVersion,
                result.downloadedSize.ToFileSizeString(), result.totalDownloadSize.ToFileSizeString(), downloadSize.ToFileSizeString() );
        }
        
        private void Awake()
        {
            this.btnConfirm.onClick.AddListener( this.OnConfirm );
            this.btnCancel.onClick.AddListener( this.OnCancel );
        }
        
        private void OnDestroy()
        {
            this.btnConfirm.onClick.RemoveListener( this.OnConfirm );
            this.btnCancel.onClick.RemoveListener( this.OnCancel );
        }
        
        private void OnConfirm()
        {
            this.gameObject.SetActive( false );
            this.confirm?.Invoke();
        }
        
        private void OnCancel()
        {
            this.gameObject.SetActive( false );
            this.cancel?.Invoke();
        }
    }
}