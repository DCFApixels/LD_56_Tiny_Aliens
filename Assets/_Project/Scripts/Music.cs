using UnityEngine;

namespace Project
{
    public class Music : MonoBehaviour
    {
        public AudioSource Source;

        public static Music Instance;
        public void Start()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Source.Play();
            DontDestroyOnLoad(gameObject);
        }
    }
}