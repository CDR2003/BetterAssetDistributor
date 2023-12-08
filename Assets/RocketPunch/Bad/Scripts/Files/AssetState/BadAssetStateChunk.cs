using System.Collections.Generic;

namespace RocketPunch.Bad
{
    public class BadAssetStateChunk
    {
        public string guid;
        
        public List<string> dependencies = new();

        public byte[] hash;

        public void Read( BadStringIndexedFile file )
        {
            this.guid = file.ReadString();
            
            var dependencyCount = file.ReadInt();
            for( var i = 0; i < dependencyCount; i++ )
            {
                this.dependencies.Add( file.ReadString() );
            }
            
            this.hash = file.ReadBytes();
        }
        
        public void Write( BadStringIndexedFile file )
        {
            file.Write( this.guid );
            file.Write( this.dependencies.Count );
            foreach( var dependency in this.dependencies )
            {
                file.Write( dependency );
            }
            file.Write( this.hash );
        }
    }
}