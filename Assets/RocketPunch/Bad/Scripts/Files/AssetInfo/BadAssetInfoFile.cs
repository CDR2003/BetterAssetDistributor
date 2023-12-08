using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace RocketPunch.Bad
{
    public class BadAssetInfoFile
    {
        public Dictionary<string, BadAssetInfoChunk> assets = new();
        
        public Dictionary<string, BadBundleInfoChunk> bundles = new();

        public static BadAssetInfoFile Create( List<BadAssetGroup> groups, string outputPath )
        {
            var file = new BadAssetInfoFile();
            foreach( var group in groups )
            {
                var bundle = new BadBundleInfoChunk();
                bundle.name = group.name;
                bundle.hash = BadHashUtility.ComputeXXHash( Path.Combine( outputPath, bundle.name ) );
                file.bundles.Add( bundle.name, bundle );
            }

            foreach( var group in groups )
            {
                foreach( var asset in group.assets )
                {
                    var chunk = new BadAssetInfoChunk();
                    chunk.name = asset.name;
                    chunk.path = asset.path;
                    chunk.guid = asset.guid;
                    chunk.bundle = group.name;
                    foreach( var dependency in asset.dependencies )
                    {
                        chunk.dependencies.Add( dependency.guid );
                    }
                    file.assets.Add( chunk.guid, chunk );
                }
            }

            return file;
        }

        public static BadAssetInfoFile ReadFromFile( string path )
        {
            using var file = new BadStringIndexedFile( path );
            var result = new BadAssetInfoFile();
            
            var bundleCount = file.ReadInt();
            for( var i = 0; i < bundleCount; i++ )
            {
                var bundle = new BadBundleInfoChunk();
                bundle.Read( file );
                result.bundles.Add( bundle.name, bundle );
            }
            
            var assetCount = file.ReadInt();
            for( var i = 0; i < assetCount; i++ )
            {
                var asset = new BadAssetInfoChunk();
                asset.Read( file );
                result.assets.Add( asset.guid, asset );
            }
            
            return result;
        }

        public void WriteToFile( string path )
        {
            using var file = new BadStringIndexedFile();
            
            file.Write( this.bundles.Count );
            foreach( var bundle in bundles )
            {
                bundle.Value.Write( file );
            }
            
            file.Write( this.assets.Count );
            foreach( var asset in assets )
            {
                asset.Value.Write( file );
            }
            
            file.WriteToFile( path );
        }
    }
}