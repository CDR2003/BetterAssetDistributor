using UnityEngine;
using UnityEngine.Networking;

namespace RocketPunch.Bad
{
    public class BadReadFileOperation : BadOperation<byte[]>
    {
        private readonly string _path;

        private UnityWebRequestAsyncOperation _request;
        
        public BadReadFileOperation( string path )
        {
            _path = path;
        }

        public override void Run()
        {
            var url = _path;
            if( _path.Contains( "://" ) == false && _path.Contains( ":///" ) == false )
            {
                url = "file://" + _path;
            }

            using var www = UnityWebRequest.Get( url );
            _request = www.SendWebRequest();
            _request.completed += this.OnRequestCompleted;
        }

        private void OnRequestCompleted( AsyncOperation obj )
        {
            _request.completed -= this.OnRequestCompleted;
            
            if( _request.webRequest.result != UnityWebRequest.Result.Success )
            {
                this.Error( _request.webRequest.error );
                return;
            }
            
            this.Complete( _request.webRequest.downloadHandler.data );
        }
    }
}