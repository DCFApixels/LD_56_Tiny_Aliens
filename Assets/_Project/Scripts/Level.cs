using System;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu]
    public class Level : ScriptableObject
    {
        public bool PatternLocked;
        public GameObject LevelPrefab;
        public Pattern Pattern;

        public RandomPlacement[] RandomPlaces;
        public StaticPlacement[] Statics;
        public AttractorPlacement[] Attractors;
        public EnemyPlacement[] Enemies;
        public RockPlacement[] Rocks;
    }

    [Serializable]
    public class RockPlacement
    {
        public Rock Prefab;
        public Vector3 Position;
        public bool RandomPosition;
        public float TimeToKill = 3f;
    }
}