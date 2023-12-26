using System.Collections.Generic;

namespace RocketPunch.Bad
{
    public class BadAssetStateFile
    {
        public Dictionary<string, BadAssetStateChunk> assets = new();

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
            var file = new BadStringIndexedFile( path );
            var assetStateFile = new BadAssetStateFile();
            var assetCount = file.ReadInt();
            for( var i = 0; i < assetCount; i++ )
            {
                var asset = new BadAssetStateChunk();
                asset.Read( file );
                assetStateFile.assets.Add( asset.guid, asset );
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

        public BadAssetModificationState CheckModification( BadAsset newAsset )
        {
            var oldAsset = this.assets.GetValueOrDefault( newAsset.guid );
            if( oldAsset == null )
            {
                return BadAssetModificationState.BrandNew;
            }
            
            if( BadHashUtility.AreEqual( oldAsset.hash, newAsset.hash ) == false )
            {
                return BadAssetModificationState.Modified;
            }

            return this.CheckDependencyModification( newAsset, oldAsset );
        }

        private BadAssetModificationState CheckDependencyModification( BadAsset newAsset, BadAssetStateChunk oldAsset )
        {
            if( newAsset.dependencies.Count != oldAsset.dependencies.Count )
            {
                return BadAssetModificationState.Modified;
            }
            
            foreach( var newDependency in newAsset.dependencies )
            {
                var state = this.CheckModification( newDependency );
                if( state != BadAssetModificationState.Identical )
                {
                    return BadAssetModificationState.Modified;
                }
            }

            return BadAssetModificationState.Identical;
        }
    }
}