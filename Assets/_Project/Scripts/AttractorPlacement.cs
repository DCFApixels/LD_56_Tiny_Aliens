using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project
{
    [Serializable]
    public class AttractorPlacement
    {
        public Attractor Attractor;
        public Vector3 Position;
        public bool FollowRoute;
        [FormerlySerializedAs("RouteAmount")] public int RoutePointsAmount;
        public Vector3[] RoutePoints;
    }
}