namespace RocketPunch.Bad
{
    public class BadBundleStateChunk
    {
        public string name;
        
        public byte[] hash;

        public int size;
        
        public void Read( BadStringIndexedFile file )
        {
            this.name = file.ReadString();
            this.hash = file.ReadBytes();
            this.size = file.ReadInt();
        }
        
        public void Write( BadStringIndexedFile file )
        {
            file.Write( this.name );
            file.Write( this.hash );
            file.Write( this.size );
        }
    }
}