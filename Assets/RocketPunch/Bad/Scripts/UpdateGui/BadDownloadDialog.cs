using UnityEngine;
using UnityEngine.UI;

namespace RocketPunch.Bad
{
    public class BadDownloadDialog : MonoBehaviour
    {
        public Text txtDownloadedSize;

        public Text txtTotalSize;

        public Text txtFilename;
        
        public Text txtProgress;
        
        public Image imgProgress;

        public BadDownloader downloader { get; set; }
        
        private void Update()
        {
            if( this.downloader == null )
                return;

            this.txtDownloadedSize.text = this.downloader.downloadedSize.ToFileSizeString();
            this.txtTotalSize.text = this.downloader.totalSize.ToFileSizeString();
            this.txtFilename.text = this.downloader.currentFilename;
            this.txtProgress.text = $"{this.downloader.progress:P1}";
            this.imgProgress.fillAmount = this.downloader.progress;
        }
    }
}