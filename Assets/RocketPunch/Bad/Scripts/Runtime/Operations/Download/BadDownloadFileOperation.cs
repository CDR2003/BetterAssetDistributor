using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketPunch.Bad
{
    public class BadDownloadFileOperation : BadOperation
    {
        private readonly string _filename;
        
        private readonly byte[] _hash;
        
        private readonly int _size;
        
        public BadDownloadFileOperation( string filename, byte[] hash, int size )
        {
            _filename = filename;
            _hash = hash;
            _size = size;
        }
        
        public override void Run()
        {
            var url = BadPathHelper.GetRemoteAssetPath( _filename );
            var request = new UnityWebRequest( url );
            request.downloadHandler = new DownloadHandlerBuffer();
            
            var operation = request.SendWebRequest();
            operation.completed += this.OnWebRequestCompleted;
        }

        private void OnWebRequestCompleted( AsyncOperation operation )
        {
            operation.completed -= this.OnWebRequestCompleted;
            
            var request = (UnityWebRequestAsyncOperation)operation;
            if( request.webRequest.result != UnityWebRequest.Result.Success )
            {
                this.Error( $"Failed to download file '{_filename}'" );
                return;
            }
            
            var data = request.webRequest.downloadHandler.data;
            if( data.Length != _size )
            {
                this.Error( $"Downloaded file size mismatch. Expected: {_size}B, Actual: {data.Length}B" );
                return;
            }

            var actualHash = BadHashUtility.ComputeXXHash( data );
            if( BadHashUtility.AreEqual( _hash, actualHash ) == false )
            {
                this.Error( $"Downloaded file hash mismatch. Expected: {_hash}, Actual: {actualHash}" );
                return;
            }
            
            var path = BadPathHelper.GetLocalDownloadPath( _filename );
            File.WriteAllBytes( path, data );
            
            this.Complete();
        }
    }
}