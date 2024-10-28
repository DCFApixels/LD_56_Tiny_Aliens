using DCFApixels.DragonECS;
using System;

namespace Project
{
    internal struct LockMovement : IEcsComponent
    {
        public LockMovementType Reason;
    }

    [Flags]
    public enum LockMovementType
    {
        None = 0,
        FromLevel = 1 << 0,
        FromRock = 1 << 1
    }
}