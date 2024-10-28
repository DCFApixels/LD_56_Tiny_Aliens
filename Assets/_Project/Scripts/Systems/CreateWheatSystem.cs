using DCFApixels.DragonECS;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Project
{
    internal class CreateWheatSystem : IEcsRun
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<GenerateWheat> GenerateWheats;
        }

        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] SceneData _sceneData;
        [EcsInject] StaticData _staticData;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var amount = a.GenerateWheats.Get(e).Size;
                var bottomLeft = (Vector2)_sceneData.SpawnCenter.position - _sceneData.Size * .5f;
                var diff = new Vector2(_sceneData.Size.x / (amount.x - 1), _sceneData.Size.y / (amount.y - 1));
                var massiveToCreate = new Vector3[amount.x * amount.y];
                var index = 0;
                for (int i = 0; i < amount.x; i++)
                {
                    for (int j = 0; j < amount.y; j++)
                    {
                        massiveToCreate[index] = new Vector3(bottomLeft.x + i * diff.x, bottomLeft.y + j * diff.y, 0f) + (Vector3)Random.insideUnitCircle * _staticData.WheatRandomMove;
                        massiveToCreate[index] = massiveToCreate[index].To2D(0);
                        index++;
                    }
                }

                Instantiate(amount, massiveToCreate);

                _world.DelEntity(e);
            }
        }

        private async void Instantiate(Vector2Int amount, Vector3[] massiveToCreate)
        {
            await Object.InstantiateAsync(_staticData.Wheat, amount.x * amount.y, _sceneData.WheatFieldRoot, massiveToCreate, ReadOnlySpan<Quaternion>.Empty);

            _sceneData.UI.UIDissolve.Play();
        }
    }
}