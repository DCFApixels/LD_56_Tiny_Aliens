using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    public class FollowRouteSystem : IEcsRun
    {
        class FollowAspect : EcsAspectAuto
        {
            [Inc] public EcsPool<TransformRef> TransformRef;
            [Inc] public EcsPool<FollowRoute> FollowRoutes;
            [Exc] public EcsPool<Delay> Delay;

        }
        class SelectFollowAspect : EcsAspectAuto
        {
            [Inc] public EcsPool<TransformRef> TransformRefs;
            [Inc] public EcsPool<FollowRoute> FollowRoutes;
            [Inc] public EcsTagPool<Select> Selected;
            [Exc] public EcsPool<LockMovement> LockMovements;
        }

        private static readonly int Walk = Animator.StringToHash("Walk");
        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] SceneData _sceneData;
        [EcsInject] StaticData _staticData;
        [EcsInject] RuntimeData _runtimeData;

        public void Run()
        {
            if (_runtimeData.State != State.Game)
            {
                return;
            }

            foreach (var e in _world.Where(out FollowAspect a))
            {
                ref var route = ref a.FollowRoutes.Get(e);
                var routePoints = route.Points;
                var targetPosition = routePoints[route.CurrentPoint];
                var transformRef = a.TransformRef.Get(e);
                var moveSpeed = route.CustomSpeed ? route.Speed : _staticData.MoveSpeed;
                Vector2 newPos = Vector2.MoveTowards(transformRef.Value.position, targetPosition, moveSpeed * Time.deltaTime);

                // Debug.Log($"MOVE SPEED: {moveSpeed}");

                var animatorRefs = _world.GetPool<AnimatorRef>();
                if (animatorRefs.Has(e))
                {
                    animatorRefs.Get(e).Value.SetBool(Walk, true);
                }

                if (newPos == (Vector2)targetPosition)
                {
                    a.Delay.Add(e).Value = route.CustomWaitTime ? route.WaitTime : _staticData.FollowRouteWaitTime;

                    if (animatorRefs.Has(e))
                    {
                        animatorRefs.Get(e).Value.SetBool(Walk, false);
                    }

                    _world.GetPool<CheckPattern>().Add(_world.NewEntity());
                    route.CurrentPoint++;
                    route.CurrentPoint %= route.Points.Length;
                }

                transformRef.Value.SetPosition2D(newPos);
            }

            foreach (var e in _world.Where(out SelectFollowAspect a))
            {
                a.FollowRoutes.Del(e);
            }
        }
    }
}