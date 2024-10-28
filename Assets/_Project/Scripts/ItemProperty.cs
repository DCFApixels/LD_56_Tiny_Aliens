using System;
using UnityEngine;

namespace Project
{
    [Serializable]
    public struct ItemProperty
    {
        public bool CantBeDragged;
        public bool IgnoreAttractor;
        public bool IgnoreInPattern;

        public bool FollowRoute;
        public int RoutePointsAmount;
        public Vector3[] RoutePoints;
    }
}