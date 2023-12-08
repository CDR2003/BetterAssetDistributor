using System.Collections.Generic;
using UnityEngine;

namespace RocketPunch.Bad
{
    [CreateAssetMenu( menuName = "BAD/Asset Group" )]
    public class BadAssetGroupDef : ScriptableObject
    {
        public List<Object> assets;
    }
}