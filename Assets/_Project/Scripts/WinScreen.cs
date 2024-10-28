using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project
{
    internal class WinScreen : MonoBehaviour
    {
        public Button NextButton;
        [SerializeField] private SceneData _sceneData;

        private void Start()
        {
            NextButton.onClick.AddListener(OnNextClick);
        }

        private async void OnNextClick()
        {

            await Awaitable.WaitForSecondsAsync(_sceneData.UI.DissolveTime);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Show(bool state)
        {
            gameObject.SetActive(true);
        }
    }
}