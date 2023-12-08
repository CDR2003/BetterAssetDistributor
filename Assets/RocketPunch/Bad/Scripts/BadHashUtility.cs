using System.IO;
using System.Linq;
using Extensions.Data;
using UnityEngine;

namespace RocketPunch.Bad
{
    public static class BadHashUtility
    {
        private static readonly XXHash64 _xxhasher = XXHash64.Create();
        
        public static byte[] ComputeXXHash( string path, out int size )
        {
            var bytes = File.ReadAllBytes( path );
            size = bytes.Length;
            return _xxhasher.ComputeHash( bytes );
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