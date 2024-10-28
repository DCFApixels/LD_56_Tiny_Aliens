using DCFApixels.DragonECS;
using DG.Tweening;
using UnityEngine;

namespace Project
{
    public class LoseOnEnter : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var itemView = other.GetComponentInParent<ItemView>();

            if (itemView && itemView.Entity.IsAlive)
            {

                var cantBeKilled = Game.World.GetPool<CantBeKilled>();
                if (cantBeKilled.Has(itemView.Entity.ID))
                {
                    Debug.Log($"Ignore Item entered lose zone because of cant be killed: {itemView.name}", gameObject);
                    return;
                }

                Debug.Log($"Item entered lose zone: {itemView.name}", gameObject);
                var staticData = Game.Instance.StaticData;
                var toolTip = Object.Instantiate(staticData.Tooltip, itemView.transform.position, Quaternion.identity);
                toolTip.Text.text = staticData.DeathMessage;
                toolTip.Text.color = staticData.DeathMessageColor;

                var sceneCamera = Game.Instance.SceneData.Camera;

                sceneCamera.transform.DOKill();
                sceneCamera.transform.DOMove(itemView.transform.position.SetZ(sceneCamera.transform.position.z), staticData.DeathCameraTime).SetEase(staticData.DeathCameraEase);
                sceneCamera.DOKill();
                DOTween.To(() => sceneCamera.orthographicSize, x => sceneCamera.orthographicSize = x, staticData.DeathCameraSize, staticData.DeathCameraTime).SetEase(staticData.DeathCameraSizeEase).SetTarget(sceneCamera);


                Game.World.GetPool<ChangeState>().Add(Game.World.NewEntity()).Value = State.Lose;
            }
        }
    }
}