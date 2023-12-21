using System;

namespace RocketPunch.Bad
{
    [Serializable]
    public struct BadVersion : IComparable<BadVersion>
    {
        public int major;
        
        public int minor;
        
        public int autoIncrement;
        
        public BadVersion( int major, int minor, int autoIncrement )
        {
            this.major = major;
            this.minor = minor;
            this.autoIncrement = autoIncrement;
        }
        
        public static BadVersion Parse( string version )
        {
            var parts = version.Split( '.' );
            return new BadVersion( int.Parse( parts[0] ), int.Parse( parts[1] ), int.Parse( parts[2] ) );
        }
        
        public BadVersion GetNextVersion()
        {
            return new BadVersion( this.major, this.minor, this.autoIncrement + 1 );
        }

        public int CompareTo( BadVersion other )
        {
            if( this.major != other.major )
            {
                return this.major.CompareTo( other.major );
            }
            if( this.minor != other.minor )
            {
                return this.minor.CompareTo( other.minor );
            }
            return this.autoIncrement.CompareTo( other.autoIncrement );
        }

        public override string ToString()
        {
            return $"{this.major}.{this.minor}.{this.autoIncrement}";
        }
    }
}