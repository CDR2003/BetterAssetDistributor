namespace RocketPunch.Bad
{
    public class BadBundleInfoChunk
    {
        public string name;
        
        public byte[] hash;

        public int size;
        
        public BadBundleLocation location;

        public void Read( BadStringIndexedFile file )
        {
            this.name = file.ReadString();
            this.hash = file.ReadBytes();
            this.size = file.ReadInt();
            this.location = (BadBundleLocation)file.ReadInt();
        }
        
        public void Write( BadStringIndexedFile file )
        {
            file.Write( this.name );
            file.Write( this.hash );
            file.Write( this.size );
            file.Write( (int)this.location );
        }
    }
}