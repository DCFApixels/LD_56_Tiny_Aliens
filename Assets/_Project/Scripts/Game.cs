using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project
{
    internal class Game : MonoBehaviour
    {
        private EcsPipeline _pipeline;
        private EcsDefaultWorld _world;
        [SerializeField]
        private RuntimeData _runtimeData;

        [FormerlySerializedAs("_sceneData")][SerializeField] public SceneData SceneData;
        [FormerlySerializedAs("_staticData")][SerializeField] public StaticData StaticData;
        public static Game Instance;
        public static EcsDefaultWorld World { get; private set; }

        void Start()
        {
            Instance = this;
            _world = new EcsDefaultWorld();
            World = _world;
            _pipeline = EcsPipeline.New()
                .Add(new InitSystem())
                .Add(new SpawnLevelSystem())
                .Add(new DelaySystem())
                .Add(new CheckPatternSystem())
                .Add(new MouseMaskSystem())

                .Add(new InputSystem())
                .Add(new RemoveSystem())
                .Add(new ChainsOnItemSystem())
                .Add(new LimitMoveOfPatternPartSystem())
                .Add(new ChangeStateSystem())
                .Add(new UpdatePatternSystem())
                .Add(new CreateWheatSystem())
                .Add(new AttractorSystem())
                .Add(new SelectSystem())
                .Add(new FollowLightSystem())
                .Add(new GenerateRouteSystem())
                .Add(new FollowRouteSystem())
                .Add(new RotationAroundSystem())
                .Add(new KillRockSystem())
                .Add(new UpdateHealthBarSystem())
                .AutoDelToEnd<Changed>()
                .AutoDelToEnd<Select>()
                .Inject(_world)
                .Inject(_runtimeData)
                .Inject(SceneData)
                .Inject(StaticData)
                .Inject(new ItemCacheService())
                .AutoInject()
                .AddUnityDebug(_world)
                .BuildAndInit();
        }

        void Update()
        {
            _pipeline.Run();
        }

        private void OnDestroy()
        {
            World = default;
            // Invoking IEcsDestroy.Destroy() on all added systems.
            _pipeline.Destroy();
            _pipeline = null;
            // Requires deleting worlds that will no longer be used.
            _world.Destroy();
            _world = null;
        }
    }
}