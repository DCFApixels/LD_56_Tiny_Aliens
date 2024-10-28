using DCFApixels.DragonECS;

namespace Project
{
    public class UpdateHealthBarSystem : IEcsRun
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<HealthBarRef> HealthBar;
            [Inc] public EcsPool<Health> Healths;
            [Inc] public EcsTagPool<Changed> Changeds;
        }

        [EcsInject] EcsDefaultWorld _world;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var health = ref a.Healths.Get(e);
                ref var healthBarRef = ref a.HealthBar.Get(e);
                if (health.Current == health.Max)
                {
                    healthBarRef.Value.gameObject.SetActive(false);
                }
                else
                {
                    healthBarRef.Value.gameObject.SetActive(true);
                }
                healthBarRef.Value.SetPercent(health.Current / health.Max);
            }
        }
    }
}