using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    public struct TransformRef : IEcsComponent
    {
        public Transform Value;
    }
}