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
            this.hash = BadHashUtility.ComputeAssetXXHash( this.path );
            this.name = Path.GetFileNameWithoutExtension( this.path );
        }

        public override string ToString()
        {
            return $"{this.name} ({this.guid})";
        }
    }
}