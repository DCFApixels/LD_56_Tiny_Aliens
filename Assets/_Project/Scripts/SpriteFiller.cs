using System;
using UnityEngine;

namespace Project
{
    public class SpriteFiller : MonoBehaviour
    {
        public Vector2 Size;
        public SpriteRenderer Prefab;
        public Sprite[] Sprites;
        public Vector2Int Amount;
        public float randomness;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireCube(transform.position, Size);
        }

        private async void Start()
        {
            var amount = Amount;
            var bottomLeft = (Vector2)transform.position - Size * .5f;
            var diff = new Vector2(Size.x / (amount.x - 1), Size.y / (amount.y - 1));
            var massiveToCreate = new Vector3[amount.x * amount.y];
            var index = 0;
            for (int i = 0; i < amount.x; i++)
            {
                for (int j = 0; j < amount.y; j++)
                {
                    massiveToCreate[index] = new Vector3(bottomLeft.x + i * diff.x, bottomLeft.y + j * diff.y, 0f) + (Vector3)UnityEngine.Random.insideUnitCircle * randomness;
                    massiveToCreate[index].z = massiveToCreate[index].y;
                    index++;
                }
            }

            try
            {
                var copies = await InstantiateAsync(Prefab, amount.x * amount.y, transform, massiveToCreate,
                    ReadOnlySpan<Quaternion>.Empty, destroyCancellationToken);

                foreach (var copy in copies)
                {
                    copy.sprite = Sprites[UnityEngine.Random.Range(0, Sprites.Length)];
                }
            }
            catch (OperationCanceledException)
            {

            }
        }
    }
}