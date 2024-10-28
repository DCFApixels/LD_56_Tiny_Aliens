using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    internal class MouseMaskSystem : IEcsRun, IEcsInit
    {
        [EcsInject] StaticData _staticData;
        [EcsInject] SceneData _sceneData;
        [EcsInject] RuntimeData _runtimeData;
        [EcsInject] EcsDefaultWorld _world;

        public void Run()
        {
            if (Input.GetMouseButton(0))
            {
                _runtimeData.MouseMask.SetActive(true);
                var position = _sceneData.Camera.ScreenToWorldPoint(Input.mousePosition).SetZ(0);
                _runtimeData.MouseMask.transform.position = position;
            }
            else
            {
                _runtimeData.MouseMask.SetActive(false);
            }
        }

        public void Init()
        {
            _runtimeData.MouseMask = Object.Instantiate(_staticData.MouseMask);
            _runtimeData.MouseMask.gameObject.SetActive(true);
        }
    }
}