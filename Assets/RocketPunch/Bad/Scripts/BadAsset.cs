using System.Collections.Generic;
using System.IO;
using Extensions.Data;

namespace RocketPunch.Bad
{
    public class BadAsset
    {
        public readonly string name;
        
        public readonly string path;

        public readonly string guid;
        
        public List<BadAsset> dependencies = new();
        
        public HashSet<BadAsset> referencers = new();

        public readonly byte[] hash;
        
        public BadAsset( string guid, string path )
        {
            this.guid = guid;
            this.path = path;
            this.hash = BadHashUtility.ComputeXXHash( this.path, out _ );
            this.name = Path.GetFileNameWithoutExtension( this.path );
        }
    }
}