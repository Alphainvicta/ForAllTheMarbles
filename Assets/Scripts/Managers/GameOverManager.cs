using UnityEngine;
using TMPro;
using System.Collections;
using System;

namespace Managers
{
    public class GameOverManager : MonoBehaviour
    {
        public GameObject gameOverMenu;
        public TextMeshProUGUI gameOverText;
        public GameObject gameOverPanel;
        public static bool? isVictory = null;
        private void OnEnable()
        {
            GameManager.GameEnd += GameOverMenu;
        }

        private void OnDisable()
        {
            GameManager.GameEnd -= GameOverMenu;
        }

        private void GameOverMenu()
        {
            StartCoroutine(GameOverStatus());
        }

        private IEnumerator GameOverStatus()
        {
            yield return new WaitUntil(() => isVictory != null);
            if (isVictory == true)
            {
                gameOverText.text = "Victory!!!";
            }
            else
            {
                gameOverText.text = "Failure";
            }
            gameOverMenu.SetActive(true);
            yield return new WaitForSeconds(3f);
            isVictory = null;
            gameOverPanel.SetActive(true);
        }
    }
}