using System.IO;
using System.Linq;
using Extensions.Data;
using UnityEngine;

namespace RocketPunch.Bad
{
    public static class BadHashUtility
    {
        private static readonly XXHash64 _xxhasher = XXHash64.Create();

        public static byte[] ComputeAssetXXHash( string path )
        {
            var fileContent = File.ReadAllBytes( path );
            var metaContent = File.ReadAllBytes( path + ".meta" );
            var content = fileContent.Concat( metaContent ).ToArray();
            return ComputeXXHash( content );
        }
        
        public static byte[] ComputeXXHash( string path, out int size )
        {
            var bytes = File.ReadAllBytes( path );
            size = bytes.Length;
            return ComputeXXHash( bytes );
        }

        public static byte[] ComputeXXHash( byte[] data )
        {
            return _xxhasher.ComputeHash( data );
        }
        
        public static string ComputeHash128( string path )
        {
            var bytes = File.ReadAllBytes( path );
            return Hash128.Compute( bytes ).ToString();
        }
        
        public static bool AreEqual( byte[] a, byte[] b )
        {
            if( a.Length != b.Length )
            {
                return false;
            }

            return a.SequenceEqual( b );
        }
    }
}