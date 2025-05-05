using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

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
        public static bool isPaused;

        [Header("Frame Settings")]
        public static float TargetFrameRate = 60.0f;

        private List<IPausable> pausableObjects = new List<IPausable>();

        public static bool tutorialActive;
        public static bool tutorialCompleted;
        public static int tutorialStep;
        public static Coroutine tutorialCoroutine;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = (int)TargetFrameRate;

            isPaused = false;

            PlayerManager playerManager = gameObject.GetComponent<PlayerManager>();
            playerManager.SetPlayerMarbles();

            string filePath = Application.persistentDataPath + "/Save.json";
            if (!System.IO.File.Exists(filePath))
            {
                SaveNewData(true);
            }

            string json = System.IO.File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            tutorialCompleted = saveData.tutorialCompleted;

            if (tutorialCompleted)
                tutorialActive = false;
            else
                tutorialActive = true;
        }

        public void RegisterPausable(IPausable pausable)
        {
            if (!pausableObjects.Contains(pausable))
            {
                pausableObjects.Add(pausable);
            }
        }

        public void UnregisterPausable(IPausable pausable)
        {
            if (pausableObjects.Contains(pausable))
            {
                pausableObjects.Remove(pausable);
            }
        }

        private void Start()
        {
            UiManager uiManager = gameObject.GetComponent<UiManager>();
            LevelManager levelManager = gameObject.GetComponent<LevelManager>();
            PlayerManager playerManager = gameObject.GetComponent<PlayerManager>();

            levelManager.LevelScriptStart();
            uiManager.UiScriptStart();
            playerManager.PlayerScriptStart();
            MenuGame();
            AudioManager.Instance.Play("MenuMusic");
        }

        public static void Menu()
        {
            MenuGame?.Invoke();
            isPaused = false;
            AudioManager.Instance.Play("MenuMusic");
            AudioManager.Instance.Stop("GameMusic");
        }

        public static void PlayGame()
        {
            if (PlayerManager.isUnlocked)
            {
                GameStart?.Invoke();
                isPaused = false;
                AudioManager.Instance.Play("GameMusic");
                AudioManager.Instance.Stop("MenuMusic");
                AudioManager.Instance.Play("StartGame");
            }
            else
            {
                PlayerManager.RefreshData();
                GameStart?.Invoke();
                isPaused = false;
                AudioManager.Instance.Play("GameMusic");
                AudioManager.Instance.Stop("MenuMusic");
                AudioManager.Instance.Play("StartGame");
            }
        }

        public static void PauseGame()
        {
            GamePaused?.Invoke();
            isPaused = true;

            Instance.PauseAllRegisteredObjects();

            AudioManager.Instance.Play("Pause");
            AudioManager.Instance.Pause("GameMusic");

            PauseAllParticles(true);
        }

        public static void UnpauseGame()
        {
            GameUnpaused?.Invoke();
            isPaused = false;

            Instance.ResumeAllRegisteredObjects();

            AudioManager.Instance.Play("Unpause");
            AudioManager.Instance.Unpause("GameMusic");

            PauseAllParticles(false);
        }

        private void PauseAllRegisteredObjects()
        {
            foreach (var pausable in pausableObjects)
            {
                pausable?.OnPause();
            }
        }

        private void ResumeAllRegisteredObjects()
        {
            foreach (var pausable in pausableObjects)
            {
                pausable?.OnResume();
            }
        }

        private static void PauseAllParticles(bool pause)
        {
            ParticleSystem[] allParticles = GameObject.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);

            foreach (ParticleSystem ps in allParticles)
            {
                if (ps == null) continue;

                if (pause)
                {
                    if (ps.isPlaying) ps.Pause();
                }
                else
                {
                    if (ps.isPaused) ps.Play();
                }
            }
        }

        public static void EndGame()
        {
            GameEnd?.Invoke();
            isPaused = false;
            AudioManager.Instance.Play("GameOver");
            AudioManager.Instance.Stop("GameMusic");
        }

        public static void Store()
        {
            StoreGame.Invoke();
            isPaused = false;
            AudioManager.Instance.Play("StoreOpen");
        }

        public static void ReloadScene()
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene);
        }

        public static void SaveNewData(bool newFile)
        {
            string filePath = Application.persistentDataPath + "/Save.json";
            List<bool> unlockedStatuses = new();
            for (int i = 0; i < PlayerManager.playerMarbles.marbles.Count; i++)
            {
                unlockedStatuses.Add(false);
            }

            unlockedStatuses[0] = true;

            SaveData saveData = new()
            {
                selectedMarbleIndex = 0,
                unlockedStatuses = unlockedStatuses,
                highScore = 0,
                tutorialCompleted = !newFile,
            };

            string json = JsonUtility.ToJson(saveData, true);
            System.IO.File.WriteAllText(filePath, json);
            Debug.Log("Data saved to: " + filePath);

            ScoreManager.highScore = 0;
        }

        public static void TutorialisCompleted()
        {
            string filePath = Application.persistentDataPath + "/Save.json";

            string json = System.IO.File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            saveData.tutorialCompleted = true;

            string updatedJson = JsonUtility.ToJson(saveData, true);
            System.IO.File.WriteAllText(filePath, updatedJson);

            tutorialCompleted = true;
            tutorialActive = false;
        }

        public static IEnumerator TutorialAction(int step)
        {

            isPaused = true;
            PlayerManager.SetPlayerInput(true);
            UiManager.uiTutorialInstance.SetActive(true);

            switch (step)
            {
                case 1:
                    tutorialStep = 1;
                    UiManager.uiTutorialScript.tutorialPanel1.SetActive(true);
                    yield return new WaitUntil(() => !isPaused);
                    UiManager.uiTutorialScript.tutorialPanel1.SetActive(false);
                    break;
                case 2:
                    tutorialStep = 2;
                    UiManager.uiTutorialScript.tutorialPanel2.SetActive(true);
                    yield return new WaitUntil(() => !isPaused);
                    UiManager.uiTutorialScript.tutorialPanel2.SetActive(false);
                    break;
                case 3:
                    tutorialStep = 3;
                    UiManager.uiTutorialScript.tutorialPanel3.SetActive(true);
                    yield return new WaitUntil(() => !isPaused);
                    UiManager.uiTutorialScript.tutorialPanel3.SetActive(false);
                    break;
                default:
                    break;
            }

            PlayerManager.SetPlayerInput(false);
            UiManager.uiTutorialInstance.SetActive(false);
            tutorialCoroutine = null;
            yield return null;
        }
    }

    public interface IPausable
    {
        void OnPause();
        void OnResume();
    }
}

[Serializable]
public class SaveData
{
    public int selectedMarbleIndex;
    public List<bool> unlockedStatuses;
    public int highScore;
    public bool tutorialCompleted;
}