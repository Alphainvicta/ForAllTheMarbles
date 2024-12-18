using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public PlayerMarble[] playerMarbles;
        public PlayerInput playerInput;
        public static event Action GameMenu;
        public static event Action GameStart;
        public static event Action GamePaused;
        public static event Action GameEnd;
        public static event Action GameStore;
        // private void Awake()
        // {
        //     DontDestroyOnLoad(gameObject);
        // }

        private void Start()
        {
            Menu();
        }

        public static void Menu()
        {
            GameMenu?.Invoke();
        }

        public static void PlayGame()
        {
            GameStart?.Invoke();
        }

        public static void PauseGame()
        {
            GamePaused?.Invoke();
        }

        public static void EndGame()
        {
            GameEnd?.Invoke();
        }

        public static void StoreGame()
        {
            GameStore.Invoke();
        }

        public static void ReloadScene()
        {
            Time.timeScale = 1f;
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene);
        }
    }
}

