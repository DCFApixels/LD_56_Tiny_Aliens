using DCFApixels.DragonECS;

namespace Project
{
    public class UpdatePatternSystem : IEcsRun
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<ItemRef> ItemPool;
            [Inc] public EcsPool<PartOfPattern> PartOfPatterns;
            [Inc] public EcsTagPool<Changed> Changed;
        }

        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] RuntimeData _runtimeData;
        [EcsInject] StaticData _staticData;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var itemView = a.ItemPool.Get(e).View;
                var index = a.PartOfPatterns.Get(e).Index;
                if (index == 0)
                {
                    for (int i = 1; i < _runtimeData.Pattern.Items.Count; i++)
                    {
                        var patternItem = _runtimeData.Pattern.Items[i];
                        var relativePosition = patternItem.Item.transform.position - itemView.transform.position;
                        if (relativePosition.magnitude < _staticData.MinPatternDistance)
                        {
                            relativePosition = relativePosition.normalized * _staticData.MinPatternDistance;
                            patternItem.Item.transform.position = itemView.transform.position + relativePosition;
                        }
                        patternItem.RelativePosition = relativePosition;
                    }
                }
                else
                {
                    _runtimeData.Pattern.Items[index].RelativePosition = itemView.transform.position -
                                                                         _runtimeData.Pattern.Items[0].Item.transform
                                                                             .position;

                    var firstElement = _runtimeData.Pattern.Items[0];
                    for (int i = 1; i < _runtimeData.Pattern.Items.Count; i++)
                    {
                        var patternItem = _runtimeData.Pattern.Items[i];
                        var relativePosition = patternItem.Item.transform.position - firstElement.Item.transform.position;
                        if (relativePosition.magnitude < _staticData.MinPatternDistance)
                        {
                            relativePosition = relativePosition.normalized * _staticData.MinPatternDistance;
                            patternItem.Item.transform.position = firstElement.Item.transform.position + relativePosition;
                        }
                        patternItem.RelativePosition = relativePosition;
                    }
                }
            }
        }
    }
}