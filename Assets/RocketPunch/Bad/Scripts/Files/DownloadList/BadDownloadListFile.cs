using System.Collections.Generic;
using System.Linq;

namespace RocketPunch.Bad
{
    public class BadDownloadListFile
    {
        public const string Filename = "download_list.bad";
        
        public string version;
        
        public List<BadDownloadItemChunk> items = new();

        public BadAssetInfoFile oldInfo { get; private set; }
        
        public BadAssetInfoFile newInfo { get; private set; }

        public static BadDownloadListFile Create( BadAssetInfoFile newInfo, BadAssetInfoFile oldInfo, string version )
        {
            var file = new BadDownloadListFile();
            file.oldInfo = oldInfo;
            file.newInfo = newInfo;
            file.version = version;
            foreach( var newBundle in newInfo.bundles )
            {
                var oldBundle = oldInfo.bundles.GetValueOrDefault( newBundle.Key );
                if( oldBundle == null || BadHashUtility.AreEqual( newBundle.Value.hash, oldBundle.hash ) == false )
                {
                    var item = new BadDownloadItemChunk();
                    item.CopyFrom( newBundle.Value );
                    file.items.Add( item );
                }
            }

            return file;
        }
        
        public static BadDownloadListFile ReadFromFile( string path )
        {
            using var file = new BadStringIndexedFile( path );
            var downloadListFile = new BadDownloadListFile();
            downloadListFile.version = file.ReadString();
            
            var itemCount = file.ReadInt();
            for( var i = 0; i < itemCount; i++ )
            {
                var item = new BadDownloadItemChunk();
                item.Read( file );
                downloadListFile.items.Add( item );
            }
            
            return downloadListFile;
        }
        
        public void WriteToFile( string path )
        {
            using var file = new BadStringIndexedFile();
            this.WriteToStringIndexFile( file );
            file.WriteToFile( path );
        }

        public void WriteToFileAsync( string path )
        {
            using var file = new BadStringIndexedFile();
            this.WriteToStringIndexFile( file );
            file.WriteToFileAsync( path );
        }

        public int CalculateTotalDownloadSize()
        {
            return this.items.Sum( i => i.size );
        }
        
        public bool HasAllDownloaded()
        {
            return this.items.All( i => i.downloaded );
        }
        
        public int CalculateDownloadedSize()
        {
            return this.items.Where( i => i.downloaded ).Sum( i => i.size );
        }
        
        private void WriteToStringIndexFile( BadStringIndexedFile file )
        {
            file.Write( this.version );
            file.Write( this.items.Count );
            foreach( var item in this.items )
            {
                item.Write( file );
            }
        }
    }
}