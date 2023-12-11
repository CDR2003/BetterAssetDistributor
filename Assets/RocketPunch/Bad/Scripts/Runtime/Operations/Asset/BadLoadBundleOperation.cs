using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketPunch.Bad.Operations
{
    public class BadLoadBundleOperation : BadOperation<AssetBundle>
    {
        private readonly BadBundleInfo _bundleInfo;
        
        private UnityWebRequestAsyncOperation _request;

        private UnityWebRequest _www;
        
        public BadLoadBundleOperation( BadBundleInfo bundleInfo )
        {
            _bundleInfo = bundleInfo;
        }
        
        public override void Run()
        {
            if( _bundleInfo.hasLoaded )
            {
                this.Complete( _bundleInfo.bundle );
                return;
            }

            _bundleInfo.state = BadBundleState.Loading;

            var path = BadPathHelper.GetLocalAssetPath( _bundleInfo.name );
            var url = path;
            if( path.Contains( "://" ) == false && path.Contains( ":///" ) == false )
            {
                url = "file://" + Path.GetFullPath( path ).Replace( "\\", "/" );
            }

            _www = UnityWebRequestAssetBundle.GetAssetBundle( url );
            _request = _www.SendWebRequest();
            _request.completed += this.OnRequestCompleted;
        }

        public override string ToString()
        {
            return $"LoadBundle: {_bundleInfo.name}";
        }

        private void OnRequestCompleted( AsyncOperation obj )
        {
            _request.completed -= this.OnRequestCompleted;
            
            if( _www.result != UnityWebRequest.Result.Success )
            {
                this.Error( _request.webRequest.error );
                return;
            }
            
            _bundleInfo.bundle = DownloadHandlerAssetBundle.GetContent( _www );
            _bundleInfo.state = BadBundleState.Loaded;
            
            _www.Dispose();
            _www = null;
            
            BadLog.Info( $"[ASYNC] Loaded bundle '{_bundleInfo.name}'" );
            
            this.Complete( _bundleInfo.bundle );
        }
    }
}