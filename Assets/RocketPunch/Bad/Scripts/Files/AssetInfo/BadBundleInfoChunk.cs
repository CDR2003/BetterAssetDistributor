namespace RocketPunch.Bad
{
    public class BadBundleInfoChunk
    {
        public string name;
        
        public byte[] hash;

        public void Read( BadStringIndexedFile file )
        {
            this.name = file.ReadString();
            this.hash = file.ReadBytes();
        }
        
        public void Write( BadStringIndexedFile file )
        {
            file.Write( this.name );
            file.Write( this.hash );
        }
    }
}