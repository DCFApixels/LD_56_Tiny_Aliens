using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace Project
{
    public class ChainsOnItemSystem : IEcsRun
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<ItemRef> Items;
            [Inc] public EcsTagPool<AddChains> AddChains;
            [Exc] public EcsPool<ChainRef> ChainRefs;
        }

        class SelecteChainedAspect : EcsAspectAuto
        {
            [Inc] public EcsPool<ItemRef> Items;
            [Inc] public EcsPool<ChainRef> ChainRefs;
            [Inc] public EcsTagPool<Select> Selectes;
        }

        [EcsInject] EcsWorld _world;
        [EcsInject] StaticData _staticData;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var itemView = a.Items.Get(e).View;
                var chain = Object.Instantiate(_staticData.ChainPrefab, itemView.transform);
                a.ChainRefs.Add(e).View = chain;
                a.AddChains.Del(e);
            }

            foreach (var e in _world.Where(out SelecteChainedAspect a))
            {
                var itemView = a.Items.Get(e).View;
                var toolTip = Object.Instantiate(_staticData.Tooltip, itemView.transform.position, quaternion.identity);
                toolTip.Text.text = "Hey, I'm stuck here!\n Move my friends!";
                Object.Destroy(toolTip.gameObject, toolTip.DeathTime);
            }
        }
    }
}