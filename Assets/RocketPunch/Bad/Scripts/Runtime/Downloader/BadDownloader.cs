using System;
using System.IO;

namespace RocketPunch.Bad
{
    public class BadDownloader
    {
        public event Action complete;

        public event Action<string> error;
        
        public string currentFilename => _file.items[_currentIndex].name;

        public int totalSize { get; private set; }

        public int downloadedSize { get; private set; }
        
        public float progress => (float)this.downloadedSize / this.totalSize;
        
        public bool isDone => this.downloadedSize == this.totalSize;
        
        private readonly BadDownloadListFile _file;
        
        private readonly BadVersionInfo _newVersion;

        private int _currentIndex;

        public BadDownloader( BadDownloadListFile file, BadVersionInfo newVersion )
        {
            _file = file;
            _newVersion = newVersion;
            _currentIndex = -1;

            this.totalSize = file.CalculateTotalDownloadSize();
            this.downloadedSize = file.CalculateDownloadedSize();
        }

        public void Start()
        {
            this.DownloadNextFile();
        }

        private void DownloadNextFile()
        {
            _currentIndex++;
            if( _currentIndex >= _file.items.Count )
            {
                this.Complete();
                return;
            }
            
            var item = _file.items[_currentIndex];
            if( item.downloaded )
            {
                this.DownloadNextFile();
                return;
            }
            
            var downloader = new BadDownloadFileOperation( item.name, item.hash, item.size );
            downloader.complete += this.OnDownloadComplete;
            downloader.error += this.OnDownloadError;
            downloader.Run();
        }

        private void OnDownloadError( BadOperation operation, string message )
        {
            operation.complete -= this.OnDownloadComplete;
            operation.error -= this.OnDownloadError;
            this.error?.Invoke( message );
            
            this.DownloadNextFile();
        }

        private void OnDownloadComplete( BadOperation operation )
        {
            operation.complete -= this.OnDownloadComplete;
            operation.error -= this.OnDownloadError;
            
            var item = _file.items[_currentIndex];
            item.downloaded = true;

            var downloadListFilePath = BadPathHelper.GetLocalDownloadPath( BadDownloadListFile.Filename );
            _file.WriteToFileAsync( downloadListFilePath );
            
            this.downloadedSize += item.size;
            this.DownloadNextFile();
        }

        private void Complete()
        {
            if( this.downloadedSize != this.totalSize || _file.HasAllDownloaded() == false )
            {
                return;
            }
            
            this.GenerateAssetInfo();
            this.GenerateVersionInfo();
            
            this.complete?.Invoke();
        }

        private void GenerateAssetInfo()
        {
            var oldInfo = _file.oldInfo;
            var newInfo = _file.newInfo;
            newInfo.UpdateBundleLocations( oldInfo );

            var filename = Path.GetFileName( _newVersion.assetInfoFilePath );
            newInfo.WriteToFile( BadPathHelper.GetLocalDownloadPath( filename ) );
        }

        private void GenerateVersionInfo()
        {
            _newVersion.assetInfoFilePath = Path.GetFileName( _newVersion.assetInfoFilePath );

            var path = BadPathHelper.GetLocalDownloadPath( BadVersionInfo.Filename );
            _newVersion.WriteToFile( path );
        }
    }
}