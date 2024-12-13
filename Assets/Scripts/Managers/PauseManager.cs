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

        private void HandleGameStart() => PauseMenu(1);
        private void HandleGamePaused() => PauseMenu(2);
        private void HandleGameEnd() => PauseMenu(3);

        private void PauseMenu(int option)
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
                        iconButtonText.text = "X";
                    }
                    else
                    {
                        iconButtonText.text = "-\n-\n-";
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