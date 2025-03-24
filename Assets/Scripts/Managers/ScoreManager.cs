using UnityEngine;
using System.Collections;

namespace Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static int score = 0;

        public static int highScore;
        public static float scoreUpdateInterval = 0.1f;
        public static Coroutine scoreCoroutine;

        private void Start()
        {
            LoadHighScore();
        }

        private void OnEnable()
        {
            GameManager.MenuGame += OnMenuGame;
            GameManager.GameStart += OnGameStart;
            GameManager.GamePaused += OnGamePaused;
            GameManager.GameUnpaused += OnGameUnpaused;
            GameManager.GameEnd += OnGameEnd;
            GameManager.StoreGame += OnStoreGame;
        }

        private void OnDisable()
        {
            GameManager.MenuGame -= OnMenuGame;
            GameManager.GameStart -= OnGameStart;
            GameManager.GamePaused -= OnGamePaused;
            GameManager.GameUnpaused -= OnGameUnpaused;
            GameManager.GameEnd -= OnGameEnd;
            GameManager.StoreGame -= OnStoreGame;
        }

        private void OnMenuGame()
        {
            score = 0;

            if (scoreCoroutine != null)
            {
                StopCoroutine(scoreCoroutine);
            }
        }

        private void OnGameStart()
        {
            score = 0;
            if (scoreCoroutine != null)
            {
                StopCoroutine(scoreCoroutine);
            }

            scoreCoroutine = StartCoroutine(ScoreDirector());
        }

        private void OnGamePaused()
        {
            Debug.Log("ScoreManager: Game Paused");
        }

        private void OnGameUnpaused()
        {
            Debug.Log("ScoreManager: Game UnPaused");
        }

        private void OnGameEnd()
        {
            if (scoreCoroutine != null)
            {
                StopCoroutine(scoreCoroutine);
            }

            if (score > highScore)
            {
                SaveHighScore();
            }
        }

        private void OnStoreGame()
        {
            score = 0;

            if (scoreCoroutine != null)
            {
                StopCoroutine(scoreCoroutine);
            }
        }

        private IEnumerator ScoreDirector()
        {
            while (true)
            {
                yield return new WaitUntil(() => !GameManager.isPaused && !UiManager.uiTransition);
                score += 1;
                UiManager.scoreText.text = score.ToString().PadLeft(7, '0');
                yield return new WaitForSeconds(scoreUpdateInterval);
            }
        }

        public void SaveHighScore()
        {
            string filePath = Application.persistentDataPath + "/Save.json";

            string json = System.IO.File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            saveData.highScore = score;
            highScore = score;

            string updatedJson = JsonUtility.ToJson(saveData, true);
            System.IO.File.WriteAllText(filePath, updatedJson);
        }

        public void LoadHighScore()
        {
            string filePath = Application.persistentDataPath + "/Save.json";

            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                highScore = saveData.highScore;
            }
            else
            {
                Debug.LogWarning("No save file found at: " + filePath);
            }
        }
    }
}