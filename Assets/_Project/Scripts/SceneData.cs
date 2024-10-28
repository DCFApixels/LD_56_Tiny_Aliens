using System;
using UnityEngine;

namespace Project
{
    [Serializable]
    internal class SceneData : MonoBehaviour
    {
        public Transform SpawnCenter;
        public Vector2 Size;
        public Transform PatternSpawnPosition;
        public UI UI;
        public float PatternZoneRadius = 5;

        public Transform Ufo;

        public bool OverrideLevel;
        public Level DebugLevelToUse;
        public Transform DragLimitCenter;
        public Vector2 DragLimitSize;
        public Camera Camera;

        public Transform UFOBeamStart;
        public Transform UFOBeamEnd;
        public float LightWidth;

        public LineRenderer StartBeamPosition;

        public Transform[] PossibleEndPositions;
        public float EndPositionVariation = 0.2f;

        public Transform WheatFieldRoot;

        private void OnValidate()
        {
            if (!SpawnCenter)
            {
                SpawnCenter = transform;
            }
        }

        private void OnDrawGizmos()
        {
            if (SpawnCenter)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(SpawnCenter.position, Size);
            }

            if (DragLimitCenter)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(DragLimitCenter.position, DragLimitSize);
            }

            if (PatternSpawnPosition)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(PatternSpawnPosition.position, PatternZoneRadius);
            }

            if (UFOBeamStart)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(UFOBeamStart.position - Vector3.right * LightWidth * .5f, UFOBeamStart.position + Vector3.right * LightWidth * .5f);
            }
        }
    }
}