using System;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public PlayerMarble[] playerMarbles;
        public static event Action GameMenu;
        public static event Action GameStart;
        public static event Action GamePaused;
        public static event Action GameEnd;
        public static event Action GameStore;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Menu();
        }

        public void PlayGame()
        {
            GameStart?.Invoke();
        }

        public void PauseGame()
        {
            GamePaused?.Invoke();
        }

        public void Menu()
        {
            GameMenu?.Invoke();
        }
    }
}

