using System;
using Object = UnityEngine.Object;

namespace RocketPunch.Bad
{
    public class BadAssetSyncUnloadTask : BadSyncLoadTask
    {
        public readonly Object obj;

        public BadAssetSyncUnloadTask( Object obj )
        {
            this.obj = obj;
        }
        
        public override void Run()
        {
            var loadedAsset = BadLoadedAssetLibrary.Get( this.obj );
            if( loadedAsset == null )
            {
                throw new Exception( $"Cannot find asset '{this.obj.name}' ({this.obj.GetType()}) in loaded asset library" );
            }
            
            loadedAsset.asset.Unload();
        }
    }
}