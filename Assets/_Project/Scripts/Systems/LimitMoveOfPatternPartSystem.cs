using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    public class LimitMoveOfPatternPartSystem : IEcsRun
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<ItemRef> Items;
            [Inc] public EcsPool<PartOfPattern> PartOfPatterns;
        }

        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] SceneData _sceneData;
        [EcsInject] RuntimeData _runtimeData;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var itemView = a.Items.Get(e).View;
                var diff = itemView.transform.position - _sceneData.PatternSpawnPosition.position;
                var clampMagnitude = Vector2.ClampMagnitude(diff, _sceneData.PatternZoneRadius);
                itemView.transform.SetPosition2D((Vector2)_sceneData.PatternSpawnPosition.position + clampMagnitude);
            }
        }
    }
}