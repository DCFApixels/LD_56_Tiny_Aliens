using DCFApixels.DragonECS;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project
{
    public class CheckPatternSystem : IEcsRun
    {
        class CheckAspect : EcsAspect
        {
            public EcsTagPool<CheckPattern> CheckPatterns = Inc;
        }

        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] RuntimeData _runtimeData;
        [EcsInject] ItemCacheService _itemCacheService;
        [EcsInject] StaticData _staticData;

        List<Collider2D> results = new();
        ItemView[] PatternItems;

        public void Run()
        {
            foreach (var e in _world.Where(out CheckAspect _))
            {
                var patternItem = _runtimeData.Pattern.Items[0];
                PatternItems = new ItemView[_runtimeData.Pattern.Items.Count];
                if (_itemCacheService.Items.TryGetValue(patternItem.Item.Id, out var entities))
                {
                    foreach (var entlong in entities)
                    {
                        var entity = entlong.ID;
                        var firstItem = _world.GetPool<ItemRef>().Get(entity);
                        Array.Clear(PatternItems, 0, PatternItems.Length);
                        PatternItems[0] = firstItem.View;
                        var matchIndices = _world.GetPool<MatchIndex>();
                        for (int i = 1; i < _runtimeData.Pattern.Items.Count; i++)
                        {
                            var nextItem = _runtimeData.Pattern.Items[i];
                            var offset = firstItem.View.transform.position + nextItem.RelativePosition;
                            var overlappedItems = Physics2D.OverlapCircle(offset, _staticData.PlacementErrorRadius,
                                _staticData.ContactFilter,
                                results);
                            if (overlappedItems > 0)
                            {
                                for (int j = 0; j < overlappedItems; j++)
                                {
                                    var collider2D = results[j];
                                    if (collider2D)
                                    {
                                        var otherItem = collider2D.GetComponentInParent<ItemView>();
                                        if (!otherItem || !otherItem.Entity.IsAlive)
                                        {
                                            continue;
                                        }

                                        var ignoreInPattern = _world.GetTagPool<IgnoreInPattern>();
                                        if (ignoreInPattern.Has(otherItem.Entity.ID))
                                        {
                                            continue;
                                        }

                                        if (otherItem.Id == nextItem.Item.Id)
                                        {
                                            if (!matchIndices.Has(otherItem.Entity.ID))
                                            {
                                                PatternItems[i] = otherItem;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (PatternItems[i] == default)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        var remove = true;
                        for (int i = 0; i < PatternItems.Length; i++)
                        {
                            if (PatternItems[i] == default)
                            {
                                remove = false;
                                break;
                            }
                        }

                        if (remove)
                        {
                            var min = Vector2.positiveInfinity;
                            var max = Vector2.negativeInfinity;

                            var sequence = DOTween.Sequence();

                            var delayPool = _world.GetPool<Delay>();
                            var cantBeKilledPool = _world.GetPool<CantBeKilled>();
                            var delay = PatternItems.Length * _staticData.PunchDuration;
                            for (var index = 0; index < PatternItems.Length; index++)
                            {
                                var itemView = PatternItems[index];

                                if (itemView.transform.position.x < min.x)
                                {
                                    min.x = itemView.transform.position.x;
                                }

                                if (itemView.transform.position.y < min.y)
                                {
                                    min.y = itemView.transform.position.y;
                                }

                                if (itemView.transform.position.x > max.x)
                                {
                                    max.x = itemView.transform.position.x;
                                }

                                if (itemView.transform.position.y > max.y)
                                {
                                    max.y = itemView.transform.position.y;
                                }

                                if (!matchIndices.Has(itemView.Entity.ID))
                                {
                                    matchIndices.Add(itemView.Entity.ID).Index = index;
                                }

                                itemView.transform.DOKill(true);
                                sequence.Append(itemView.transform.DOPunchScale(_staticData.PunchScale / 2, _staticData.PunchDuration, elasticity: _staticData.PunchFrequency).SetEase(_staticData.PunchEase));

                                SpawnWithDelay(itemView, _staticData.PunchDuration * index,
                                    _staticData.PunchPaticle);

                                if (!delayPool.Has(itemView.Entity.ID))
                                {
                                    delayPool.Add(itemView.Entity.ID).Value = delay;
                                    cantBeKilledPool.Add(itemView.Entity.ID);
                                }
                                else
                                {
                                    delayPool.Get(itemView.Entity.ID).Value = delay;
                                }
                            }

                            var center = (min + max) * 0.5f;
                            DOTween.To(() => (Vector2)_runtimeData.CurrentBeamPosition, x => _runtimeData.CurrentBeamPosition = x, center, _staticData.BeamFollowTime).SetEase(_staticData.BeamFollowEase);

                            var tooltip = Object.Instantiate(_staticData.Tooltip, center, Quaternion.identity);
                            tooltip.Text.text = "Saved";
                            tooltip.Text.color = _staticData.SavedTextColor;
                            tooltip.Text.fontStyle = FontStyles.UpperCase;
                            Object.Destroy(tooltip.gameObject, tooltip.DeathTime);
                        }

                    }
                }

                _world.DelEntity(e);
            }
        }

        private async void SpawnWithDelay(ItemView itemView, float delay, GameObject go)
        {
            await Awaitable.WaitForSecondsAsync(delay);
            Object.Instantiate(go, itemView.transform.position, quaternion.identity);
        }
    }

    public struct Delay : IEcsComponent
    {
        public float Value;
    }
}