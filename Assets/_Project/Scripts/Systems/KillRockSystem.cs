using DCFApixels.DragonECS;
using DG.Tweening;
using UnityEngine;

namespace Project
{
    public class KillRockSystem : IEcsRun
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<RockRef> Rock;
            [Inc] public EcsPool<Health> Healths;
        }

        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] RuntimeData _runtimeData;
        [EcsInject] StaticData _staticData;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var rock = a.Rock.Get(e).Value;
                if (Vector2.Distance(rock.transform.position, _runtimeData.CurrentBeamPosition) <
                    rock.Radius)
                {
                    ref var health = ref a.Healths.Get(e);
                    health.Current -= Time.deltaTime;

                    var changed = _world.GetPool<Changed>();
                    if (!changed.Has(e))
                    {
                        changed.Add(e);
                    }
                    if (health.Current <= 0)
                    {
                        rock.transform.DOKill();
                        rock.transform.DOScale(Vector3.zero, _staticData.RockDeathDuration).OnComplete(() 
                            => Object.Destroy(rock.gameObject));
                        _world.DelEntity(e);
                    }
                }
            }
        }
    }
}