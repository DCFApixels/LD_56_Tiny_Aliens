using DCFApixels.DragonECS;

namespace Project
{
    internal struct GenerateRoute : IEcsComponent
    {
        public int PointAmount;

        public bool CustomSpeed;
        public float Speed;
        public bool CustomWaitTime;
        public float WaitTime;
    }
}