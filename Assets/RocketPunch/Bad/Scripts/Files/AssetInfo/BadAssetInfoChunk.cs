using System.Collections.Generic;

namespace RocketPunch.Bad
{
    public class BadAssetInfoChunk
    {
        public string name;

        public string path;
        
        public string guid;
        
        public List<string> dependencies = new();
        
        public string bundle;

        public void Read( BadStringIndexedFile file )
        {
            this.name = file.ReadString();
            this.path = file.ReadString();
            this.guid = file.ReadString();
            var dependencyCount = file.ReadInt();
            for( var i = 0; i < dependencyCount; i++ )
            {
                this.dependencies.Add( file.ReadString() );
            }
            this.bundle = file.ReadString();
        }

        public void Write( BadStringIndexedFile file )
        {
            file.Write( this.name );
            file.Write( this.path );
            file.Write( this.guid );
            file.Write( this.dependencies.Count );
            foreach( var dependency in this.dependencies )
            {
                file.Write( dependency );
            }
            file.Write( this.bundle );
        }
    }
}