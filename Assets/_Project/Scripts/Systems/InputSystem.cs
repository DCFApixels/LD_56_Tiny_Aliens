using DCFApixels.DragonECS;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    internal class InputSystem : IEcsRun
    {
        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] RuntimeData _runtimeData;
        [EcsInject] StaticData _staticData;
        [EcsInject] SceneData _sceneData;

        ItemView _draggingItemView;
        Vector3 Offset;
        List<Collider2D> results = new();

        public void Run()
        {
            if (_runtimeData.State != State.Game) { return; }



            if (Input.GetMouseButtonDown(0))
            {
                var pointInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                var find = Physics2D.OverlapPoint(pointInWorld, _staticData.ContactFilter, results);
                for (int i = 0; i < find; i++)
                {
                    var rock = results[i].GetComponentInParent<Rock>();
                    if (rock != null) { return; }
                }

                for (int i = 0; i < find; i++)
                {
                    var item = results[i].GetComponentInParent<ItemView>();
                    if (item && item.Entity.IsAlive)
                    {
                        var locked = _world.GetPool<LockMovement>();
                        _world.GetTagPool<Select>().Add(item.Entity.ID);
                        if (!locked.Has(item.Entity.ID))
                        {
                            _draggingItemView = item;
                            if (item.TakeSound)
                            {
                                AudioSource.PlayClipAtPoint(item.TakeSound, Vector3.zero);
                            }

                            Offset = (pointInWorld - _draggingItemView.transform.position).SetZ(0);
                            break;
                        }
                    }
                }
            }
            else if (Input.GetMouseButton(0))
            {
                var pointInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (pointInWorld.Inside(_sceneData.DragLimitCenter.position, _sceneData.DragLimitSize))
                {
                    _sceneData.Ufo.transform.DOKill();
                    DOTween.To(() => (Vector2)_runtimeData.CurrentBeamPosition, x => _runtimeData.CurrentBeamPosition = x, (Vector2)pointInWorld, _staticData.BeamFollowTime).SetTarget(_runtimeData).SetEase(_staticData.BeamFollowEase);
                    _sceneData.Ufo.transform.DOMoveX(pointInWorld.x, _staticData.UfoFlyTime).SetEase(_staticData.UfoEase);
                }

                if (_draggingItemView != null)
                {
                    var before = _draggingItemView.transform.position;
                    var newPosition = pointInWorld.SetZ(0) - Offset;
                    var dragLimitCenter = _sceneData.DragLimitCenter.position;
                    var patternPool = _world.GetPool<PartOfPattern>();

                    if (!patternPool.Has(_draggingItemView.Entity.ID))
                    {
                        newPosition.x = Mathf.Clamp(
                            newPosition.x,
                            dragLimitCenter.x - _sceneData.DragLimitSize.x * .5f,
                            dragLimitCenter.x + _sceneData.DragLimitSize.x * .5f);
                        newPosition.y = Mathf.Clamp(
                            newPosition.y,
                            dragLimitCenter.y - _sceneData.DragLimitSize.y * .5f,
                            dragLimitCenter.y + _sceneData.DragLimitSize.y * .5f);
                    }

                    _draggingItemView.transform.SetPosition2D(newPosition, 3);

                    if (before != newPosition)
                    {
                        _world.GetPool<Changed>().Add(_draggingItemView.Entity.ID);
                    }
                }
            }
            else
            {
                if (_draggingItemView != null)
                {
                    var checkEntity = _world.NewEntity();
                    _world.GetTagPool<CheckPattern>().Add(checkEntity);
                }
                _draggingItemView = default;
            }
        }
    }
}