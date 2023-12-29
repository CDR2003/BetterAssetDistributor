using System;
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

        public static string GetFilename( string versionId )
        {
            return $"asset_info_{versionId}.bad";
        }

        public static BadAssetInfoFile Create( List<BadAssetGroup> groups, Dictionary<string, BadBundleStateChunk> bundleStates, string bundlePath )
        {
            var file = new BadAssetInfoFile();
            
            foreach( var group in groups )
            {
                var bundle = new BadBundleInfoChunk();
                
                var path = Path.Join( bundlePath, group.name );
                if( File.Exists( path ) )
                {
                    bundle.name = group.name;
                    bundle.hash = BadHashUtility.ComputeXXHash( path, out bundle.size );
                }
                else
                {
                    var oldBundle = bundleStates.GetValueOrDefault( group.name );
                    if( oldBundle == null )
                    {
                        throw new Exception( $"Cannot find old bundle '{group.name}' from asset state" );
                    }
                    
                    bundle.name = oldBundle.name;
                    bundle.hash = oldBundle.hash;
                    bundle.size = oldBundle.size;
                }
                
                bundle.location = BadBundleLocation.Local;
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
            return ReadFromFile( file );
        }

        public static BadAssetInfoFile ReadFromFile( byte[] content )
        {
            using var file = new BadStringIndexedFile( content );
            return ReadFromFile( file );
        }

        public static BadAssetInfoFile ReadFromFile( BadStringIndexedFile file )
        {
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

        public void UpdateBundleLocations( BadAssetInfoFile baseFile )
        {
            foreach( var pair in this.bundles )
            {
                var name = pair.Key;
                var bundle = pair.Value;
                var baseBundle = baseFile.bundles.GetValueOrDefault( name );
                if( baseBundle != null && BadHashUtility.AreEqual( bundle.hash, baseBundle.hash ) )
                {
                    bundle.location = baseBundle.location;
                }
                else
                {
                    bundle.location = BadBundleLocation.Download;
                }
            }
        }
    }
}