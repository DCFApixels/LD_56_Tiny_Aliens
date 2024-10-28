using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    public class RotationAroundSystem : IEcsRun
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<RotateInfo> RotateInfos;
        }

        [EcsInject] EcsDefaultWorld _world;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var rotateInfo = a.RotateInfos.Get(e);
                var rootLocalEulerAngles = rotateInfo.Root.localEulerAngles;
                rootLocalEulerAngles.z += rotateInfo.Speed * Time.deltaTime;
                rotateInfo.Root.localEulerAngles = rootLocalEulerAngles;
            }
        }
    }
}