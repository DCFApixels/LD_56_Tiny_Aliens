using DCFApixels.DragonECS;
using UnityEngine.Rendering;

namespace Project
{
    internal class SelectSystem : IEcsRun
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<ItemRef> Items;
            [Inc] public EcsTagPool<Select> Selected;
        }

        [EcsInject] EcsWorld _world;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var itemView = a.Items.Get(e).View;
                itemView.ChangeSortLayer("Foreground");
                var sortingGroups = itemView.GetComponentsInChildren<SortingGroup>();
                foreach (var sortingGroup in sortingGroups)
                {
                    sortingGroup.sortingLayerName = "Foreground";
                }
            }
        }
    }
}