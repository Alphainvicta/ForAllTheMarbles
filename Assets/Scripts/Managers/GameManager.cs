using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static event Action MenuGame;
        public static event Action GameStart;
        public static event Action GamePaused;
        public static event Action GameUnpaused;
        public static event Action GameEnd;
        public static event Action StoreGame;

        [Header("Frame Settings")]
        public static float TargetFrameRate = 60.0f;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = (int)TargetFrameRate;

            string filePath = Application.persistentDataPath + "/Save.json";
            if (!System.IO.File.Exists(filePath))
            {
                List<bool> unlockedStatuses = new();
                PlayerManager playerManager = FindFirstObjectByType<PlayerManager>();
                for (int i = 0; i < playerManager.playerMarbles.marbles.Count; i++)
                {
                    unlockedStatuses.Add(playerManager.playerMarbles.marbles[i].isUnlocked);
                }

                SaveData saveData = new()
                {
                    selectedMarbleIndex = 0,
                    unlockedStatuses = unlockedStatuses
                };

                string json = JsonUtility.ToJson(saveData, true);
                System.IO.File.WriteAllText(filePath, json);
                Debug.Log("Data saved to: " + filePath);
            }
        }

        private void Start()
        {
            MenuGame();
        }

        public static void Menu()
        {
            MenuGame?.Invoke();
        }

        public static void PlayGame()
        {
            GameStart?.Invoke();
        }

        public static void PauseGame()
        {
            GamePaused?.Invoke();
        }

        public static void UnpauseGame()
        {
            GameUnpaused?.Invoke();
        }

        public static void EndGame()
        {
            GameEnd?.Invoke();
        }

        public static void Store()
        {
            StoreGame.Invoke();
        }

        public static void ReloadScene()
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene);
        }
    }
}

[Serializable]
public class SaveData
{
    public int selectedMarbleIndex;
    public List<bool> unlockedStatuses;
}

