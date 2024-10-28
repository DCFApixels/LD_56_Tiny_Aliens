using System;
using UnityEngine;

namespace Project
{
    [Serializable]
    public class EnemyPlacement
    {
        public GameObject Prefab;
        public Vector3 Position;
        public bool FollowRoute;
        public int RoutePointsAmount;
        public Vector3[] RoutePoints;

        public float Speed;
        public float WaitTime;
    }
}