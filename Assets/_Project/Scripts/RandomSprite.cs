using UnityEngine;

namespace Project
{
    public class RandomSprite : MonoBehaviour
    {
        public SpriteRenderer SpriteRenderer;
        public Sprite[] Sprites;
        private void Awake()
        {
            var range = UnityEngine.Random.Range(0, Sprites.Length);
            if (range == 1)
            {
                SpriteRenderer.maskInteraction = SpriteMaskInteraction.None;
            }
            SpriteRenderer.sprite = Sprites[range];
        }

        private void OnValidate()
        {
            if (!SpriteRenderer)
            {
                SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }
    }
}