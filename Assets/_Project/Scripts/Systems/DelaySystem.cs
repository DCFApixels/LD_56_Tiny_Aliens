using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    public class DelaySystem : IEcsRun
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<Delay> Delays;
        }

        [EcsInject] EcsDefaultWorld _world;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                a.Delays.Get(e).Value -= Time.deltaTime;
                if (a.Delays.Get(e).Value <= 0)
                {
                    a.Delays.Del(e);
                }
            }
        }
    }
}