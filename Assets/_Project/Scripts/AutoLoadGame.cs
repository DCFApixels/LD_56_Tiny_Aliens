using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project
{
    public class AutoLoadGame : MonoBehaviour
    {
        private void Start()
        {
            Resources.UnloadUnusedAssets();
            SceneManager.LoadScene("Game");
        }
    }
}