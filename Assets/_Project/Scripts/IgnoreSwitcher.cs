using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(Collider2D))]
    public class IgnoreSwitcher : MonoBehaviour
    {
        public bool IgnoreOnEnterState;
        public bool IgnoreOnExitState;
        private void OnTriggerEnter2D(Collider2D other)
        {
            var itemView = other.GetComponentInParent<ItemView>();

            if (itemView && itemView.Entity.IsAlive)
            {
                Debug.Log($"Item entered: {itemView.name}", itemView.gameObject);
                if (IgnoreOnEnterState)
                {
                    var ign = Game.World.GetTagPool<IgnoreInPattern>();
                    if (!ign.Has(itemView.Entity.ID))
                    {
                        ign.Add(itemView.Entity.ID);
                    }
                }
                else
                {
                    var ign = Game.World.GetTagPool<IgnoreInPattern>();
                    if (ign.Has(itemView.Entity.ID))
                    {
                        ign.Del(itemView.Entity.ID);
                    }
                }

            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var itemView = other.GetComponentInParent<ItemView>();

            if (itemView && itemView.Entity.IsAlive)
            {
                Debug.Log($"Item exited: {itemView.name}", itemView.gameObject);
                if (IgnoreOnExitState)
                {
                    var ign = Game.World.GetTagPool<IgnoreInPattern>();
                    if (!ign.Has(itemView.Entity.ID))
                    {
                        ign.Add(itemView.Entity.ID);
                    }
                }
                else
                {
                    var ign = Game.World.GetTagPool<IgnoreInPattern>();
                    if (ign.Has(itemView.Entity.ID))
                    {
                        ign.Del(itemView.Entity.ID);
                    }
                }

            }
        }
    }
}