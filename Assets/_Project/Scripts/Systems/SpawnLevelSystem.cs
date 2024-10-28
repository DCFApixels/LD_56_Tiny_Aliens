using DCFApixels.DragonECS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

namespace Project
{
    internal class SpawnLevelSystem : IEcsRun
    {
        class Aspect : EcsAspect
        {
            public EcsPool<SpawnLevel> Spawn = Inc;
        }

        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] ItemCacheService _itemCacheService;
        [EcsInject] SceneData _sceneData;
        [EcsInject] RuntimeData _runtimeData;
        [EcsInject] StaticData _staticData;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var spawnLevel = a.Spawn.Get(e);
                _runtimeData.Level = spawnLevel.Level;
                _runtimeData.Pattern = Object.Instantiate(spawnLevel.Level.Pattern);
                _world.GetPool<ChangeState>().Add(_world.NewEntity()).Value = State.Game;

                if (_runtimeData.Level.LevelPrefab)
                {
                    _runtimeData.LevelView = Object.Instantiate(_runtimeData.Level.LevelPrefab);
                }

                SpawnPattern(_runtimeData.Pattern);

                foreach (var pair in spawnLevel.Level.RandomPlaces)
                {
                    for (int i = 0; i < pair.Amount; i++)
                    {
                        var randomPosition = new Vector3
                        (
                            _sceneData.SpawnCenter.position.x +
                            Random.Range(-_sceneData.Size.x / 2, _sceneData.Size.x / 2),
                            _sceneData.SpawnCenter.position.y +
                            Random.Range(-_sceneData.Size.y / 2, _sceneData.Size.y / 2),
                            0
                        );
                        var prefab = pair.Item;

                        SpawnItem(prefab, randomPosition, pair.ItemProperty);
                    }
                }

                foreach (var staticPlacement in spawnLevel.Level.Statics)
                {
                    SpawnItem(staticPlacement.Item, staticPlacement.Position, staticPlacement.ItemProperty);
                }

                foreach (var attractorPlacement in spawnLevel.Level.Attractors)
                {
                    SpawnAttractor(attractorPlacement);
                }

                foreach (var enemyPlacement in spawnLevel.Level.Enemies)
                {
                    SpawnEnemy(enemyPlacement);
                }

                foreach (var rock in spawnLevel.Level.Rocks)
                {
                    SpawnRock(rock);
                }


                _world.DelEntity(e);
            }
        }

        private void SpawnRock(RockPlacement placement)
        {
            var entity = _world.NewEntity();
            var prefab = placement.Prefab;
            var placementPosition = placement.Position;
            if (placement.RandomPosition)
            {
                placementPosition.x = _sceneData.SpawnCenter.transform.position.x + Random.Range(-_sceneData.Size.x / 2f, _sceneData.Size.x / 2f);
                placementPosition.y = _sceneData.SpawnCenter.transform.position.y + Random.Range(-_sceneData.Size.y / 2f, _sceneData.Size.y / 2f);
            }

            var instance = Object.Instantiate(
                prefab, 
                placementPosition.To2D(Consts.RockHeight), 
                Quaternion.identity);
            _world.GetPool<TransformRef>().Add(entity).Value = instance.transform;
            _world.GetPool<HealthBarRef>().Add(entity).Value = Object.Instantiate(_staticData.HealthBar, instance.HealthBarPosition);
            _world.GetPool<RockRef>().Add(entity).Value = instance;
            ref var health = ref _world.GetPool<Health>().Add(entity);
            health.Current = health.Max = placement.TimeToKill;
            _world.GetPool<Changed>().Add(entity);
        }

        private void SpawnEnemy(EnemyPlacement placement)
        {
            var entity = _world.NewEntity();
            var prefab = placement.Prefab;
            var instance = Object.Instantiate(
                prefab, 
                placement.Position.To2D(Consts.AlienHeight), 
                Quaternion.identity);
            _world.GetPool<TransformRef>().Add(entity).Value = instance.transform;
            _world.GetPool<AnimatorRef>().Add(entity).Value = instance.GetComponentInChildren<Animator>(true);

            if (placement.FollowRoute)
            {
                if (placement.RoutePoints.Length < 2)
                {
                    ref var generateRoute = ref _world.GetPool<GenerateRoute>().Add(entity);
                    generateRoute.PointAmount = placement.RoutePointsAmount;
                    generateRoute.CustomSpeed = placement.Speed != 0;
                    generateRoute.Speed = placement.Speed;
                    generateRoute.CustomWaitTime = placement.WaitTime != 0;
                    generateRoute.WaitTime = placement.WaitTime;
                }
                else
                {
                    ref var followRoute = ref _world.GetPool<FollowRoute>().Add(entity);
                    followRoute.Points = placement.RoutePoints;
                    followRoute.CustomSpeed = placement.Speed != 0;
                    followRoute.Speed = placement.Speed;
                    followRoute.CustomWaitTime = placement.WaitTime != 0;
                    followRoute.WaitTime = placement.WaitTime;
                }
            }
        }

        private void SpawnAttractor(AttractorPlacement attractorPlacement)
        {
            var entity = _world.NewEntity();
            var prefab = attractorPlacement.Attractor;
            var instance = Object.Instantiate(
                prefab, 
                attractorPlacement.Position.To2D(Consts.AlienHeight), 
                Quaternion.identity);
            instance.Entity = (_world, entity);
            _world.GetPool<TransformRef>().Add(entity).Value = instance.transform;
            _world.GetPool<AttractorRef>().Add(entity).View = instance;

            if (attractorPlacement.FollowRoute)
            {
                if (attractorPlacement.RoutePoints.Length < 2)
                {
                    _world.GetPool<GenerateRoute>().Add(entity).PointAmount = attractorPlacement.RoutePointsAmount;
                }
                else
                {
                    _world.GetPool<FollowRoute>().Add(entity).Points = attractorPlacement.RoutePoints;
                }
            }

            if (instance.RotationSpeed != 0)
            {
                ref var rotateInfos = ref _world.GetPool<RotateInfo>().Add(entity);
                rotateInfos.Root = instance.Root;
                rotateInfos.Speed = instance.RotationSpeed;
            }
        }

        private void SpawnItem(ItemView prefab, Vector3 position, ItemProperty itemProperty)
        {
            var itemEntity = _world.NewEntity();
            var itemRefs = _world.GetPool<ItemRef>();
            var itemView = Object.Instantiate(
                prefab, 
                position.To2D(Consts.AlienHeight), 
                Quaternion.identity);
            itemView.Entity = (_world, itemEntity);
            itemRefs.Add(itemEntity).View = itemView;

            _world.GetPool<TransformRef>().Add(itemEntity).Value = itemView.transform;

            if (itemProperty.CantBeDragged)
            {
                _world.GetPool<LockMovement>().Add(itemEntity).Reason |= LockMovementType.FromLevel;
                _world.GetPool<AddChains>().Add(itemEntity);
            }

            if (itemProperty.IgnoreAttractor)
            {
                _world.GetPool<IgnoreAttractor>().Add(itemEntity);
            }

            if (itemProperty.IgnoreInPattern)
            {
                _world.GetTagPool<IgnoreInPattern>().Add(itemEntity);
            }

            if (itemProperty.FollowRoute)
            {
                if (itemProperty.RoutePoints.Length < 2)
                {
                    _world.GetPool<GenerateRoute>().Add(itemEntity).PointAmount = itemProperty.RoutePointsAmount;
                }
                else
                {
                    _world.GetPool<FollowRoute>().Add(itemEntity).Points = itemProperty.RoutePoints;
                }
            }

            if (!_itemCacheService.Items.TryGetValue(prefab.Id, out var list))
            {
                list = new List<entlong>();
                _itemCacheService.Items[prefab.Id] = list;
            }

            list.Add(_world.GetEntityLong(itemEntity));
        }

        private void SpawnPattern(Pattern pattern)
        {
            var patternSpawnPosition = _sceneData.PatternSpawnPosition;
            var min = Vector2.positiveInfinity;
            var max = Vector2.negativeInfinity;
            foreach (var item in pattern.Items)
            {
                if (item.Random)
                {
                    item.RelativePosition = new Vector3(Random.Range(item.Min.x, item.Max.x), Random.Range(item.Min.y, item.Max.y), 0);
                }
            }

            foreach (var item in pattern.Items)
            {
                if (item.RelativePosition.x < min.x)
                {
                    min.x = item.RelativePosition.x;
                }

                if (item.RelativePosition.y < min.y)
                {
                    min.y = item.RelativePosition.y;
                }

                if (item.RelativePosition.x > max.x)
                {
                    max.x = item.RelativePosition.x;
                }

                if (item.RelativePosition.y > max.y)
                {
                    max.y = item.RelativePosition.y;
                }
            }

            var center = -(min + max) * 0.5f;
            var changedPool = _world.GetTagPool<Changed>();

            for (var index = 0; index < pattern.Items.Count; index++)
            {
                var patternItem = pattern.Items[index];
                var item = Object.Instantiate(patternItem.Item,
                    patternSpawnPosition.position + patternItem.RelativePosition + (Vector3)center, Quaternion.identity,
                    _sceneData.Ufo.transform);

                foreach (var itemSpriteRenderer in item.SpriteRenderers)
                {
                    itemSpriteRenderer.material = _staticData.PatternMaterial;
                    itemSpriteRenderer.sortingOrder = _staticData.PatternSortingOrder;
                    itemSpriteRenderer.sortingLayerName = _staticData.PatternSortingLayerName;
                }

                item.transform.localScale = _staticData.PatternScale;
                var patternItemEntity = _world.NewEntity();
                _world.GetPool<TransformRef>().Add(patternItemEntity).Value = item.transform;
                _world.GetPool<ItemRef>().Add(patternItemEntity).View = item;
                _world.GetTagPool<IgnoreAttractor>().Add(patternItemEntity);
                _world.GetPool<PartOfPattern>().Add(patternItemEntity).Index = index;
                if (_runtimeData.Level.PatternLocked)
                {
                    _world.GetPool<LockMovement>().Add(patternItemEntity).Reason |= LockMovementType.FromLevel;
                }

                item.Entity = (_world, patternItemEntity);
                pattern.Items[index].Item = item;

                changedPool.Add(patternItemEntity);
            }
        }
    }

    internal struct AnimatorRef : IEcsComponent
    {
        public Animator Value;
    }
}