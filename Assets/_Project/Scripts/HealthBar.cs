using UnityEngine;

namespace Project
{
    public class HealthBar : MonoBehaviour
    {
        public SpriteRenderer Bar;
        public float Width;

        public void SetPercent(float percent)
        {
            var size = Bar.size;
            size.x = percent * Width;
            Bar.size = size;
        }
    }
}