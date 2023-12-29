using System.Collections.Generic;

namespace RocketPunch.Bad
{
    public class BadAssetStateFile
    {
        public Dictionary<string, BadAssetStateChunk> assets = new();

        public Dictionary<string, BadBundleStateChunk> bundles = new();

        public static string GetFilename( string versionId )
        {
            return $"asset_state_{versionId}.bad";
        }

        public static BadAssetStateFile Create( List<BadAssetGroup> groups )
        {
            var file = new BadAssetStateFile();
            foreach( var group in groups )
            {
                foreach( var asset in group.assets )
                {
                    file.AddAsset( asset );
                }
            }
            return file;
        }

        public static BadAssetStateFile ReadFromFile( string path )
        {
            using var file = new BadStringIndexedFile( path );
            var assetStateFile = new BadAssetStateFile();
            
            var assetCount = file.ReadInt();
            for( var i = 0; i < assetCount; i++ )
            {
                var asset = new BadAssetStateChunk();
                asset.Read( file );
                assetStateFile.assets.Add( asset.guid, asset );
            }
            
            var bundleCount = file.ReadInt();
            for( var i = 0; i < bundleCount; i++ )
            {
                var bundle = new BadBundleStateChunk();
                bundle.Read( file );
                assetStateFile.bundles.Add( bundle.name, bundle );
            }
            
            return assetStateFile;
        }
        
        public void WriteToFile( string path )
        {
            using var file = new BadStringIndexedFile();
            
            file.Write( this.assets.Count );
            foreach( var pair in this.assets )
            {
                pair.Value.Write( file );
            }
            
            file.Write( this.bundles.Count );
            foreach( var pair in this.bundles )
            {
                pair.Value.Write( file );
            }
            
            file.WriteToFile( path );
        }

        public void AddAsset( BadAsset asset )
        {
            var chunk = this.assets.GetValueOrDefault( asset.guid );
            if( chunk != null )
            {
                return;
            }

            chunk = new BadAssetStateChunk();
            chunk.guid = asset.guid;
            chunk.hash = asset.hash;
            this.assets.Add( chunk.guid, chunk );

            foreach( var dependency in asset.dependencies )
            {
                this.AddAsset( dependency );
                chunk.dependencies.Add( dependency.guid );
            }
        }

        public void CombineBundleStates( Dictionary<string, BadBundleStateChunk> newBundleStates )
        {
            foreach( var pair in newBundleStates )
            {
                this.bundles[pair.Key] = pair.Value;
            }
        }
    }
}