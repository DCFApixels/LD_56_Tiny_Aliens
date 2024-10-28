using DCFApixels.DragonECS;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    internal class AttractorSystem : IEcsRun
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<AttractorRef> Attractors;
            [Exc] EcsTagPool<NotWorking> NotWorkings;
        }

        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] StaticData _staticData;

        List<Collider2D> results = new();

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var attractorRef = a.Attractors.Get(e);
                var attractorRefView = attractorRef.View;
                var hits = Physics2D.OverlapCircle(attractorRefView.transform.position, attractorRefView.Radius,
                    _staticData.ContactFilter, results);
                for (int i = 0; i < hits; i++)
                {
                    var collider2D = results[i];
                    var itemView = collider2D.GetComponentInParent<ItemView>();
                    if (!itemView)
                    {
                        continue;
                    }
                    var entity = itemView.Entity;
                    if (!entity.IsAlive)
                    {
                        continue;
                    }

                    var partOfPatterns = _world.GetPool<IgnoreAttractor>();
                    if (partOfPatterns.Has(entity.ID))
                    {
                        continue;
                    }

                    var diff = itemView.transform.position - attractorRefView.transform.position;
                    var distance = diff.magnitude;
                    if (distance < attractorRefView.Radius && distance > attractorRefView.MinRadius)
                    {
                        itemView.transform.SetPosition2D(Vector2.MoveTowards(itemView.transform.position,
                            attractorRefView.transform.position, Time.deltaTime * attractorRefView.MoveSpeed));
                    }
                }
            }
        }
    }
}