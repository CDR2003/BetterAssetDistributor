using System.Collections.Generic;

namespace RocketPunch.Bad
{
    public static class BadModificationChecker
    {
        public static List<BadAsset> CheckForModifiedAssets( List<BadAssetGroup> newGroups, string assetStateFilePath )
        {
            var file = BadAssetStateFile.ReadFromFile( assetStateFilePath );
            var modifiedAssets = new List<BadAsset>();
            foreach( var newGroup in newGroups )
            {
                foreach( var newAsset in newGroup.assets )
                {
                    var state = file.CheckModification( newAsset );
                    if( state != BadAssetModificationState.Identical )
                    {
                        modifiedAssets.Add( newAsset );
                    }
                }
            }

            return modifiedAssets;
        }
    }
}