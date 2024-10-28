using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    public class Attractor : MonoBehaviour
    {
        public entlong Entity;

        public float Radius = 5;
        public float MinRadius = 1;
        public float MoveSpeed = 5;

        public Transform Root;
        public float RotationSpeed = 360;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, Radius);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, MinRadius);
        }
    }
}