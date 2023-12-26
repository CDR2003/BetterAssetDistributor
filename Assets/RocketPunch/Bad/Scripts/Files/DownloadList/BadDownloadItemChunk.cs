namespace RocketPunch.Bad
{
    public class BadDownloadItemChunk
    {
        public string name;
        
        public byte[] hash;

        public int size;

        public bool downloaded;
        
        public void Read( BadStringIndexedFile file )
        {
            this.name = file.ReadString();
            this.hash = file.ReadBytes();
            this.size = file.ReadInt();
            this.downloaded = file.ReadBool();
        }
        
        public void Write( BadStringIndexedFile file )
        {
            file.Write( this.name );
            file.Write( this.hash );
            file.Write( this.size );
            file.Write( this.downloaded );
        }
        
        public void CopyFrom( BadBundleInfoChunk bundle )
        {
            this.name = bundle.name;
            this.hash = bundle.hash;
            this.size = bundle.size;
            this.downloaded = false;
        }

        public override string ToString()
        {
            var downloadIndicator = this.downloaded ? "✓" : "✗";
            return $"[{downloadIndicator}] {this.name}: {this.size}B";
        }
    }
}