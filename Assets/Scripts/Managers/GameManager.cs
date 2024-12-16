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

        public void Menu()
        {
            GameMenu?.Invoke();
        }

        public void PlayGame()
        {
            GameStart?.Invoke();
        }

        public void PauseGame()
        {
            GamePaused?.Invoke();
        }

        public void EndGame()
        {
            GameEnd?.Invoke();
        }

        public void StoreGame()
        {
            GameStore.Invoke();
        }

        public void ReloadScene()
        {
            Time.timeScale = 1f;
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene);
        }
    }
}

