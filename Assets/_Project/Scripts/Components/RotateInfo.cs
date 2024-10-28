using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    public struct RotateInfo : IEcsComponent
    {
        public Transform Root;
        public float Speed;
    }
}