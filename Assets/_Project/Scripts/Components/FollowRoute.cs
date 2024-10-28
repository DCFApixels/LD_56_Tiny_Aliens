using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    internal struct FollowRoute : IEcsComponent
    {
        public int CurrentPoint;
        public Vector3[] Points;
        public bool CustomSpeed;
        public float Speed;
        public bool CustomWaitTime;
        public float WaitTime;
    }
}