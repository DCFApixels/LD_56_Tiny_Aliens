using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    public class GenerateRouteSystem : IEcsRun
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<GenerateRoute> GenerateRoutes;
            [Exc] public EcsPool<FollowRoute> FollowRoutes;
        }

        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] SceneData _sceneData;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var generateRoute = a.GenerateRoutes.Get(e);
                var list = new Vector3[generateRoute.PointAmount];
                Debug.Log($"Generate route system: {generateRoute.PointAmount} {generateRoute.CustomWaitTime} {generateRoute.CustomSpeed}");
                for (int i = 0; i < generateRoute.PointAmount; i++)
                {
                    var center = _sceneData.SpawnCenter.position;
                    var size = _sceneData.Size;
                    var randomPoint = new Vector3(
                        Random.Range(center.x - size.x * .5f, center.x + size.x * .5f), Random.Range(center.y - size.y * .5f, center.y + size.y * .5f), 0);

                    list[i] = randomPoint;
                }

                ref var followRoute = ref a.FollowRoutes.Add(e);
                followRoute.Points = list;
                followRoute.CustomWaitTime = generateRoute.CustomWaitTime;
                followRoute.WaitTime = generateRoute.WaitTime;
                followRoute.CustomSpeed = generateRoute.CustomSpeed;
                followRoute.Speed = generateRoute.Speed;

                a.GenerateRoutes.Del(e);
            }
        }
    }
}