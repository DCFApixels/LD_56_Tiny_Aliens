namespace DCFApixels.DragonECS
{
    internal static class DeleteOneFrameTagComponentSystemExtensions
    {
        public static EcsPipeline.Builder AutoDel<TComponent>(this EcsPipeline.Builder b, string layerName = null)
            where TComponent : struct, IEcsTagComponent
        {
            if (EcsOneFrameComponentConsts.AUTO_DEL_LAYER == layerName)
            {
                b.Layers.InsertAfter(EcsConsts.POST_END_LAYER, EcsOneFrameComponentConsts.AUTO_DEL_LAYER);
            }
            b.AddUnique(new DeleteOneFrameTagComponentSystem<TComponent>(), layerName);
            return b;
        }
        public static EcsPipeline.Builder AutoDelToEnd<TComponent>(this EcsPipeline.Builder b)
            where TComponent : struct, IEcsTagComponent
        {
            b.Layers.InsertAfter(EcsConsts.POST_END_LAYER, EcsOneFrameComponentConsts.AUTO_DEL_LAYER);
            b.AddUnique(new DeleteOneFrameTagComponentSystem<TComponent>(), EcsOneFrameComponentConsts.AUTO_DEL_LAYER);
            return b;
        }
    }
}