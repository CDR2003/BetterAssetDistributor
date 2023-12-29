using System;
using System.Collections.Generic;
using System.IO;

namespace RocketPunch.Bad
{
    public class BadStringIndexedFile : IDisposable
    {
        private List<string> _strings;

        private MemoryStream _content;

        private BinaryWriter _writer;

        private BinaryReader _reader;

        public BadStringIndexedFile()
        {
            _strings = new List<string>();
            _content = new MemoryStream();
            _writer = new BinaryWriter( _content );
        }
        
        public BadStringIndexedFile( string path )
        {
            var file = File.Open( path, FileMode.Open );
            var fileReader = new BinaryReader( file );
            this.ReadFile( fileReader );
            fileReader.Close();
            file.Close();
        }

        public BadStringIndexedFile( byte[] content )
        {
            using var stream = new MemoryStream( content );
            var fileReader = new BinaryReader( stream );
            this.ReadFile( fileReader );
            fileReader.Close();
        }

        public void Dispose()
        {
            _content?.Dispose();
            _writer?.Dispose();
            _reader?.Dispose();
        }

        public void Write( bool value )
        {
            _writer.Write( value );
        }

        public void Write( int value )
        {
            _writer.Write( value );
        }

        public void Write( string value )
        {
            var index = _strings.IndexOf( value );
            if( index < 0 )
            {
                index = _strings.Count;
                _strings.Add( value );
            }

            _writer.Write( index );
        }

        public void Write( byte[] value )
        {
            _writer.Write( value.Length );
            _writer.Write( value );
        }

        public void WriteToFile( string path )
        {
            var file = File.Open( path, FileMode.Create );
            var fileWriter = new BinaryWriter( file );
            fileWriter.Write( _strings.Count );
            foreach( var str in _strings )
            {
                fileWriter.Write( str );
            }
            
            fileWriter.Write( _content.GetBuffer(), 0, (int)_content.Length );
            fileWriter.Close();
            file.Close();
        }

        public void WriteToFileAsync( string path )
        {
            var file = new FileStream( path, FileMode.Create );
            var fileWriter = new BinaryWriter( file );
            fileWriter.Write( _strings.Count );
            foreach( var str in _strings )
            {
                fileWriter.Write( str );
            }
            
            var task = file.WriteAsync( _content.GetBuffer(), 0, (int)_content.Length );
            task.GetAwaiter().OnCompleted( () =>
            {
                fileWriter.Close();
                file.Close();
            } );
        }

        public bool ReadBool()
        {
            return _reader.ReadBoolean();
        }
        
        public int ReadInt()
        {
            return _reader.ReadInt32();
        }
        
        public string ReadString()
        {
            var index = _reader.ReadInt32();
            return _strings[index];
        }

        public byte[] ReadBytes()
        {
            var length = _reader.ReadInt32();
            return _reader.ReadBytes( length );
        }

        private void ReadFile( BinaryReader fileReader )
        {
            var count = fileReader.ReadInt32();
            _strings = new List<string>( count );
            for( var i = 0; i < count; ++i )
            {
                _strings.Add( fileReader.ReadString() );
            }

            _content = new MemoryStream( fileReader.ReadBytes( (int)( fileReader.BaseStream.Length - fileReader.BaseStream.Position ) ) );
            _reader = new BinaryReader( _content );
        }
    }
}