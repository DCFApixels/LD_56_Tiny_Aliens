using DCFApixels.DragonECS;
using System;
using UnityEngine;

namespace Project
{
    internal class InitSystem : IEcsInit
    {
        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] StaticData _staticData;
        [EcsInject] RuntimeData _runtimeData;
        [EcsInject] SceneData _sceneData;

        public void Init()
        {
            var spawnEntity = _world.NewEntity();

            if (_sceneData.OverrideLevel && Application.isEditor)
            {
                _runtimeData.CurrentLevel = Array.IndexOf(_staticData.Levels, _sceneData.DebugLevelToUse);
                _world.GetPool<SpawnLevel>().Add(spawnEntity).Level = _sceneData.DebugLevelToUse;
            }
            else
            {
                var currentLevel = Mathf.Clamp(Profile.CurrentLevel, 0, _staticData.Levels.Length - 1);
                _runtimeData.CurrentLevel = currentLevel;
                _world.GetPool<SpawnLevel>().Add(spawnEntity).Level = _staticData.Levels[currentLevel];
            }

            _world.GetPool<GenerateWheat>().Add(_world.NewEntity()).Size = new Vector2Int((int)(_sceneData.Size.x * _staticData.WheatDensity.x), (int)(_sceneData.Size.y * _staticData.WheatDensity.y));
        }
    }
}