using DCFApixels.DragonECS;
using DG.Tweening;
using UnityEngine;

namespace Project
{
    internal class RemoveSystem : IEcsRun
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<ItemRef> Item;
            [Inc] public EcsPool<MatchIndex> MatchIndices;
            [Exc] EcsTagPool<IgnoreInPattern> IgnoreInPatterns;
            [Exc] EcsPool<Delay> delays;
        }

        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] StaticData _staticData;
        [EcsInject] ItemCacheService _itemCacheService;
        [EcsInject] SceneData _sceneData;
        [EcsInject] RuntimeData _runtimeData;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var lockMovements = _world.GetPool<LockMovement>();

                if (!lockMovements.Has(e))
                {
                    lockMovements.Add(e);
                }

                var ignoreInPattern = _world.GetPool<IgnoreInPattern>();
                if (!ignoreInPattern.Has(e))
                {
                    ignoreInPattern.Add(e);
                }

                var itemView = a.Item.Get(e).View;
                var patternIndex = a.MatchIndices.Get(e).Index;

                var patternItem = _runtimeData.Pattern.Items[patternIndex].Item;

                var delay = _staticData.BeamFollowTime;
                itemView.transform.SetParent(_sceneData.Ufo, true);
                itemView.transform.DOKill();
                var sequence = DOTween.Sequence()
                    .Append(itemView.transform.DOLocalMove(_sceneData.UFOBeamStart.localPosition, _staticData.DeathDuration).SetDelay(delay).SetEase(_staticData.DeathEase))
                    .Insert(0, itemView.transform.DOScale(Vector3.zero, _staticData.DeathDuration).SetEase(_staticData.UfoSuccingScaleEase))
                    .Insert(0, itemView.transform.DORotate(new Vector3(0, 0, Random.Range(-360, 360)), _staticData.DeathDuration));


                foreach (var itemViewSpriteRenderer in itemView.SpriteRenderers)
                {
                    itemViewSpriteRenderer.sortingLayerName = "Default";
                    itemViewSpriteRenderer.sortingOrder = 1;
                }

                sequence
                    .OnComplete(
                    () =>
                    {
                        itemView.transform.position = patternItem.transform.position;
                        itemView.transform.rotation = Quaternion.identity;
                        itemView.transform.localScale = _staticData.PatternScale;

                        //var sequence = Sequence.Create();
                        //sequence.Chain(
                        //    Tween.Scale(itemView.transform, Vector3.one, _staticData.AfterTeleportPunch));
                        //sequence.Chain(
                        //    Tween.LocalPosition(itemView.transform,
                        //        _sceneData.PossibleEndPositions[Random.Range(0, _sceneData.PossibleEndPositions.Length)].localPosition +
                        //        (Vector3)(Random.insideUnitCircle * _sceneData.EndPositionVariation), _staticData.EndPositionWalkTime,
                        //        Ease.Linear));
                        itemView.transform.DOKill();
                        var sequence = DOTween.Sequence();
                        sequence.Append(itemView.transform.DOScale(Vector3.one, _staticData.AfterTeleportPunch));
                        sequence.Append(itemView.transform.DOLocalMove(
                            _sceneData.PossibleEndPositions[Random.Range(0, _sceneData.PossibleEndPositions.Length)].localPosition +
                            (Vector3)(Random.insideUnitCircle * _sceneData.EndPositionVariation), _staticData.EndPositionWalkTime).SetEase(Ease.Linear));

                        foreach (var itemViewSpriteRenderer in itemView.SpriteRenderers)
                        {
                            itemViewSpriteRenderer.sortingLayerName = _staticData.PatternSortingLayerName;
                            itemViewSpriteRenderer.sortingOrder = _staticData.PatternSortingOrder + 1;
                        }
                    });

                var chains = _world.GetPool<ChainRef>();
                if (chains.Has(e))
                {
                    Object.Destroy(chains.Get(e).View.gameObject);
                    chains.Del(e);
                }

                var followPath = _world.GetPool<FollowRoute>();
                if (followPath.Has(e))
                {
                    followPath.Del(e);
                }

                var entlongs = _itemCacheService.Items[itemView.Id];
                for (var index = entlongs.Count - 1; index >= 0; index--)
                {
                    var entlong = entlongs[index];
                    if (entlong == (_world, e))
                    {
                        entlongs.RemoveAt(index);
                    }
                }

                if (entlongs.Count == 0)
                {
                    _itemCacheService.Items.Remove(itemView.Id);

                    if (_itemCacheService.Items.Count == 0)
                    {
                        var delayEntity = _world.NewEntity();
                        _world.GetPool<ChangeState>().Add(delayEntity).Value = State.Win;
                        _world.GetPool<Delay>().Add(delayEntity).Value = _staticData.BeforeWinDelay;
                    }
                }

                //_world.DelEntity(e);
            }
        }
    }
}