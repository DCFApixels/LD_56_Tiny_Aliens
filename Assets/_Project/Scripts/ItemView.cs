using DCFApixels.DragonECS;
using System.Linq;
using UnityEngine;

namespace Project
{
    [SelectionBase]
    public class ItemView : MonoBehaviour
    {
        public SpriteRenderer[] SpriteRenderers;
        public entlong Entity;
        public string Id;
        public AudioClip TakeSound;

        [ContextMenu(nameof(Validate))]
        private void Validate()
        {
            Id = name;

            SpriteRenderers = GetComponentsInChildren<SpriteRenderer>().Where(o => o.gameObject.layer == gameObject.layer).ToArray();
        }

        public void ChangeSortLayer(string foreground)
        {
            foreach (var spriteRenderer in SpriteRenderers)
            {
                spriteRenderer.sortingLayerName = foreground;
            }
        }
    }
}