using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketPunch.Bad
{
    public class BadReadFileOperation : BadOperation<byte[]>
    {
        private readonly string _path;

        private UnityWebRequestAsyncOperation _request;
        
        private UnityWebRequest _www;

        private string _url;
        
        public BadReadFileOperation( string path )
        {
            _path = path;
        }

        public override void Run()
        {
            _url = _path;
            if( _path.Contains( "://" ) == false && _path.Contains( ":///" ) == false )
            {
                _url = "file://" + Path.GetFullPath( _path );
            }

            _url = _url.Replace( "\\", "/" );

            _www = UnityWebRequest.Get( _url );
            _request = _www.SendWebRequest();
            _request.completed += this.OnRequestCompleted;
        }

        private void OnRequestCompleted( AsyncOperation obj )
        {
            _request.completed -= this.OnRequestCompleted;
            
            if( _www.result != UnityWebRequest.Result.Success )
            {
                this.Error( $"Read file failed: '{_url}': {_www.error}" );
                return;
            }
            
            this.Complete( _www.downloadHandler.data );
            
            _www.Dispose();
            _www = null;
        }
    }
}