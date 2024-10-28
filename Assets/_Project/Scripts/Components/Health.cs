using DCFApixels.DragonECS;

namespace Project
{
    internal struct Health : IEcsComponent
    {
        public float Current;
        public float Max;
    }
}