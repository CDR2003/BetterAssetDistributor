using System.Collections.Generic;
using System.Linq;

namespace RocketPunch.Bad
{
    public class BadDownloadListFile
    {
        public List<BadDownloadItemChunk> items = new();
        
        public static string GetFilename( string versionId )
        {
            return $"download_list_{versionId}.bad";
        }

        public static BadDownloadListFile Create( BadAssetInfoFile newInfo, BadAssetInfoFile oldInfo )
        {
            var file = new BadDownloadListFile();
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
            var file = new BadStringIndexedFile( path );
            var downloadListFile = new BadDownloadListFile();
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
            file.Write( this.items.Count );
            foreach( var item in this.items )
            {
                item.Write( file );
            }
            file.WriteToFile( path );
        }

        public int CalculateTotalDownloadSize()
        {
            return this.items.Sum( i => i.size );
        }
        
        public int CalculateDownloadedSize()
        {
            return this.items.Where( i => i.downloaded ).Sum( i => i.size );
        }
    }
}