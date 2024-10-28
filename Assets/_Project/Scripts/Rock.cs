using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    public class Rock : MonoBehaviour
    {
        public Transform HealthBarPosition;
        public float Radius;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var itemView = other.GetComponentInParent<ItemView>();

            if (itemView && itemView.Entity.IsAlive)
            {
                Debug.Log($"Item entered rock: {itemView.name}", gameObject);
                var cantBeKilled = Game.World.GetPool<CantBeKilled>();
                if (!cantBeKilled.Has(itemView.Entity.ID))
                {
                    cantBeKilled.Add(itemView.Entity.ID);
                }

                var lockMovements = Game.World.GetPool<LockMovement>();
                if (!lockMovements.Has(itemView.Entity.ID))
                {
                    lockMovements.Add(itemView.Entity.ID).Reason |= LockMovementType.FromRock;
                }
                else
                {
                    lockMovements.Get(itemView.Entity.ID).Reason |= LockMovementType.FromRock;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var itemView = other.GetComponentInParent<ItemView>();

            if (itemView && itemView.Entity.IsAlive)
            {
                Debug.Log($"Item exited rock: {itemView.name}", gameObject);

                var cantBeKilled = Game.World.GetPool<CantBeKilled>();
                if (cantBeKilled.Has(itemView.Entity.ID))
                {
                    cantBeKilled.Del(itemView.Entity.ID);
                }

                var lockMovements = Game.World.GetPool<LockMovement>();
                if (lockMovements.Has(itemView.Entity.ID))
                {
                    ref var lockMovement = ref lockMovements.Get(itemView.Entity.ID);
                    lockMovement.Reason &= ~LockMovementType.FromRock;

                    if (lockMovement.Reason == LockMovementType.None)
                    {
                        lockMovements.Del(itemView.Entity.ID);
                    }
                }
            }
        }
    }
}