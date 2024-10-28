using UnityEngine;

namespace Project
{
    public class RandomMesh : MonoBehaviour
    {
        public MeshFilter MeshFilter;
        public Mesh[] Meshes;
        private void Awake()
        {
            var index = UnityEngine.Random.Range(0, Meshes.Length);
            MeshFilter.mesh = Meshes[index];
        }

        private void OnValidate()
        {
            if (!MeshFilter)
            {
                MeshFilter = GetComponentInChildren<MeshFilter>();
            }
        }
    }
}