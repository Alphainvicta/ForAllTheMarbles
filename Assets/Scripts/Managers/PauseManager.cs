using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class PauseManager : MonoBehaviour
    {
        public GameObject pausedMenu;
        public TextMeshProUGUI iconButtonText;
        public GameObject menuPanel;
        private bool isPaused = false;
        private void OnEnable()
        {
            GameManager.GameStart += HandleGameStart;
            GameManager.GamePaused += HandleGamePaused;
            GameManager.GameEnd += HandleGameEnd;
            GameManager.GameMenu += HandleGameEnd;
        }

        private void OnDisable()
        {
            GameManager.GameStart -= HandleGameStart;
            GameManager.GamePaused -= HandleGamePaused;
            GameManager.GameEnd -= HandleGameEnd;
            GameManager.GameMenu -= HandleGameEnd;
        }

        private void HandleGameStart() => PauseGameMenu(1);
        private void HandleGamePaused() => PauseGameMenu(2);
        private void HandleGameEnd() => PauseGameMenu(3);

        private void PauseGameMenu(int option)
        {
            switch (option)
            {
                case 1:
                    pausedMenu.SetActive(true);
                    break;
                case 2:
                    isPaused = !isPaused;
                    menuPanel.SetActive(isPaused);
                    if (isPaused)
                    {
                        iconButtonText.text = "x";
                        Time.timeScale = 0f;
                    }
                    else
                    {
                        iconButtonText.text = "-\n-\n-";
                        Time.timeScale = 1f;
                    }
                    break;
                case 3:
                    pausedMenu.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
}