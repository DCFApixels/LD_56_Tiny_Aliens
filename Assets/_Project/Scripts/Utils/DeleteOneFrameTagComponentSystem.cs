using System.Collections.Generic;

namespace DCFApixels.DragonECS
{
    [MetaTags(MetaTags.HIDDEN)]
    [MetaColor(MetaColor.Grey)]
    internal class DeleteOneFrameTagComponentSystem<TComponent> : IEcsRun, IEcsInject<EcsWorld>
        where TComponent : struct, IEcsTagComponent
    {
        private sealed class Aspect : EcsAspect
        {
            public EcsTagPool<TComponent> pool = Inc;
        }
        private readonly List<EcsWorld> _worlds = new List<EcsWorld>();
        public void Inject(EcsWorld obj) => _worlds.Add(obj);
        public void Run()
        {
            for (int i = 0, iMax = _worlds.Count; i < iMax; i++)
            {
                EcsWorld world = _worlds[i];
                if (world.IsComponentTypeDeclared<TComponent>())
                {
                    foreach (var e in world.Where(out Aspect a))
                    {
                        a.pool.Del(e);
                    }
                }
            }
        }
    }
}