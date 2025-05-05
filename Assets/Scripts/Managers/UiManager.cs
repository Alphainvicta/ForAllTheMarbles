using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private GameObject uiMenuPrefab;
        [SerializeField] private GameObject uiMenuTutorialPrefab;
        [SerializeField] private GameObject uiGamePrefab;
        [SerializeField] private GameObject uiPausedPrefab;
        [SerializeField] private GameObject uiEndPrefab;
        [SerializeField] private GameObject uiConfigPrefab;
        [SerializeField] private GameObject uiTutorialPrefab;

        private GameObject uiMenuInstance;
        public static UIMenu uiMenuScript;
        private GameObject uiMenuTutorialInstance;
        public static UIMenuTutorial uiMenuTutorialScript;
        private GameObject uiGameInstance;
        public static UIGame uiGameScript;
        private GameObject uiPausedInstance;
        public static UIPause uiPauseScript;
        private GameObject uiEndInstance;
        public static UIEndGame uiEndGameScript;
        private GameObject uiConfigInstance;
        public static UIConfig uiConfigScript;
        public static GameObject uiTutorialInstance;
        public static UITutorial uiTutorialScript;

        public static bool uiTransition;
        private Coroutine beginLevelCountDownCoroutine;
        PlayerManager playerManager;

        public void UiScriptStart()
        {
            if (uiMenuInstance == null)
            {
                uiMenuInstance = Instantiate(uiMenuPrefab, Vector3.zero, Quaternion.identity);
                if (GameManager.tutorialActive)
                    uiMenuInstance.SetActive(false);
                else
                    uiMenuInstance.SetActive(true);

                uiMenuScript = uiMenuInstance.GetComponent<UIMenu>();

                playerManager = gameObject.GetComponent<PlayerManager>();

                AssignMenuButtons();
            }

            if (uiMenuTutorialInstance == null)
            {
                uiMenuTutorialInstance = Instantiate(uiMenuTutorialPrefab, Vector3.zero, Quaternion.identity);
                if (GameManager.tutorialActive)
                    uiMenuTutorialInstance.SetActive(true);
                else
                    uiMenuTutorialInstance.SetActive(false);

                uiMenuTutorialScript = uiMenuTutorialInstance.GetComponent<UIMenuTutorial>();

                AssignMenuTutorialButtons();
            }

            if (uiGameInstance == null)
            {
                uiGameInstance = Instantiate(uiGamePrefab, Vector3.zero, Quaternion.identity);
                uiGameInstance.SetActive(false);

                uiGameScript = uiGameInstance.GetComponent<UIGame>();

                AssignGameButtons();
            }
            if (uiPausedInstance == null)
            {
                uiPausedInstance = Instantiate(uiPausedPrefab, Vector3.zero, Quaternion.identity);
                uiPausedInstance.SetActive(false);

                uiPauseScript = uiPausedInstance.GetComponent<UIPause>();

                AssignPauseButtons();
            }
            if (uiEndInstance == null)
            {
                uiEndInstance = Instantiate(uiEndPrefab, Vector3.zero, Quaternion.identity);
                uiEndInstance.SetActive(false);

                uiEndGameScript = uiEndInstance.GetComponent<UIEndGame>();

                AssignEndButtons();
            }
            if (uiConfigInstance == null)
            {
                uiConfigInstance = Instantiate(uiConfigPrefab, Vector3.zero, Quaternion.identity);
                uiConfigInstance.SetActive(false);

                uiConfigScript = uiConfigInstance.GetComponent<UIConfig>();

                AssignConfigMenuButtons();
            }
            if (uiTutorialInstance == null)
            {
                uiTutorialInstance = Instantiate(uiTutorialPrefab, Vector3.zero, Quaternion.identity);
                uiTutorialInstance.SetActive(false);

                uiTutorialScript = uiTutorialInstance.GetComponent<UITutorial>();
            }
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
            if (GameManager.tutorialActive)
            {
                uiMenuInstance.SetActive(false);
                uiMenuTutorialInstance.SetActive(true);
            }
            else
            {
                uiMenuInstance.SetActive(true);
                uiMenuTutorialInstance.SetActive(false);
            }

            uiGameInstance.SetActive(false);
            uiPausedInstance.SetActive(false);
            uiEndInstance.SetActive(false);
            uiConfigInstance.SetActive(false);

            if (beginLevelCountDownCoroutine != null)
            {
                StopCoroutine(beginLevelCountDownCoroutine);
            }
        }

        private void OnGameStart()
        {
            if (beginLevelCountDownCoroutine != null)
            {
                StopCoroutine(beginLevelCountDownCoroutine);
            }

            uiMenuInstance.SetActive(false);
            uiMenuTutorialInstance.SetActive(false);

            if (GameManager.tutorialActive)
            {
                uiGameInstance.SetActive(false);
            }
            else
            {
                uiGameInstance.SetActive(true);
                beginLevelCountDownCoroutine = StartCoroutine(BeginLevelCountDown());
            }

            uiPausedInstance.SetActive(false);
            uiEndInstance.SetActive(false);
            uiConfigInstance.SetActive(false);
        }

        private void OnGamePaused()
        {
            uiMenuInstance.SetActive(false);
            uiMenuTutorialInstance.SetActive(false);
            uiGameInstance.SetActive(false);
            uiPausedInstance.SetActive(true);
            uiEndInstance.SetActive(false);
            uiConfigInstance.SetActive(false);
        }

        private void OnGameUnpaused()
        {
            uiMenuInstance.SetActive(false);
            uiMenuTutorialInstance.SetActive(false);
            uiGameInstance.SetActive(true);
            uiPausedInstance.SetActive(false);
            uiEndInstance.SetActive(false);
            uiConfigInstance.SetActive(false);
        }

        private void OnGameEnd()
        {
            uiMenuInstance.SetActive(false);
            uiMenuTutorialInstance.SetActive(false);
            uiGameInstance.SetActive(false);
            uiPausedInstance.SetActive(false);
            uiEndInstance.SetActive(true);
            uiConfigInstance.SetActive(false);

            if (beginLevelCountDownCoroutine != null)
            {
                StopCoroutine(beginLevelCountDownCoroutine);
            }
        }

        private void OnStoreGame()
        {
            uiMenuInstance.SetActive(false);
            uiMenuTutorialInstance.SetActive(false);
            uiGameInstance.SetActive(false);
            uiPausedInstance.SetActive(false);
            uiEndInstance.SetActive(false);
            uiConfigInstance.SetActive(true);

            if (beginLevelCountDownCoroutine != null)
            {
                StopCoroutine(beginLevelCountDownCoroutine);
            }
        }

        private void AssignMenuButtons()
        {
            uiMenuScript.playButton.onClick.AddListener(() => GameManager.PlayGame());
            uiMenuScript.skinsButton.onClick.AddListener(() => EnableSkinMenu());
            uiMenuScript.configButton.onClick.AddListener(() => EnableConfigMenu());

            uiMenuScript.rightButton.onClick.AddListener(() => playerManager.NextPlayerMarble(true));
            uiMenuScript.leftButton.onClick.AddListener(() => playerManager.NextPlayerMarble(false));
            uiMenuScript.menuButton.onClick.AddListener(() => CloseSkinMenu());
        }

        private void AssignMenuTutorialButtons()
        {
            uiMenuTutorialScript.playButton.onClick.AddListener(() => GameManager.PlayGame());
        }

        private void AssignGameButtons()
        {
            uiGameScript.pauseButton.onClick.AddListener(() => GameManager.PauseGame());
            uiGameScript.scoreText.gameObject.SetActive(false);
        }

        private void AssignPauseButtons()
        {
            uiPauseScript.unpauseButton.onClick.AddListener(() => GameManager.UnpauseGame());
            uiPauseScript.replayButton.onClick.AddListener(() => EnablePauseRestartPanel());
            uiPauseScript.playButton.onClick.AddListener(() => GameManager.UnpauseGame());
            uiPauseScript.menuButton.onClick.AddListener(() => EnablePauseMenuPanel());
            uiPauseScript.configButton.onClick.AddListener(() => EnableConfigPause());

            uiPauseScript.confirmRestartButton.onClick.AddListener(() => RestartGame());
            uiPauseScript.cancelRestartButton.onClick.AddListener(() => EnablePauseRestartPanel());

            uiPauseScript.confirmMenuButton.onClick.AddListener(() => BackToMenu());
            uiPauseScript.cancelMenuButton.onClick.AddListener(() => EnablePauseMenuPanel());

        }

        private void AssignConfigMenuButtons()
        {
            uiConfigScript.menuButton.onClick.AddListener(() => EnableConfigMenu());
            uiConfigScript.deleteButton.onClick.AddListener(() => EnableConfigConfirmMenu());

            uiConfigScript.cancelButton.onClick.AddListener(() => EnableConfigConfirmMenu());
            uiConfigScript.confirmButton.onClick.AddListener(() => DeleteData());

            uiConfigScript.deleteButton.gameObject.SetActive(true);
        }

        private void AssignEndButtons()
        {
            uiEndGameScript.replayButton.onClick.AddListener(() => ReplayGame());
            uiEndGameScript.menuButton.onClick.AddListener(() => GameManager.Menu());
        }

        private void EnableSkinMenu()
        {
            if (uiMenuScript.skinsPanel.activeSelf)
            {
                uiMenuScript.menuPanel.SetActive(true);
                uiMenuScript.skinsPanel.SetActive(false);

                PlayerManager.playerMarbles.SavePlayerMarbles(PlayerManager.marbleIndex);
            }
            else
            {
                uiMenuScript.menuPanel.SetActive(false);
                uiMenuScript.skinsPanel.SetActive(true);
                uiMenuScript.marbleNameText.text = PlayerManager.playerMarbles.marbles[PlayerManager.marbleIndex].isUnlocked ? PlayerManager.playerMarbles.marbles[PlayerManager.marbleIndex].marblePrefab.name : "Locked";
                uiMenuScript.marbleImage.sprite = PlayerManager.playerMarbles.marbles[PlayerManager.marbleIndex].isUnlocked ? PlayerManager.playerMarbles.marbles[PlayerManager.marbleIndex].marbleImage : PlayerManager.marbleLockedSpriteStatic;
            }
        }

        private void EnableConfigMenu()
        {
            if (uiConfigInstance.activeSelf)
            {
                uiMenuScript.menuPanel.SetActive(true);
                uiConfigInstance.SetActive(false);
            }
            else
            {
                uiConfigScript.menuButton.onClick.RemoveAllListeners();
                uiConfigScript.menuButton.onClick.AddListener(() => EnableConfigMenu());
                uiConfigScript.deleteButton.gameObject.SetActive(true);

                uiMenuScript.menuPanel.SetActive(false);
                uiConfigInstance.SetActive(true);
            }
        }

        private void EnableConfigConfirmMenu()
        {
            if (uiConfigScript.confirmPanel.activeSelf)
            {
                uiConfigScript.configPanel.SetActive(true);
                uiConfigScript.confirmPanel.SetActive(false);
            }
            else
            {
                uiConfigScript.configPanel.SetActive(false);
                uiConfigScript.confirmPanel.SetActive(true);
            }
        }

        private void EnablePauseMenuPanel()
        {
            if (uiPauseScript.backToMenuPanel.activeSelf)
            {
                uiPauseScript.pausePanel.SetActive(true);
                uiPauseScript.backToMenuPanel.SetActive(false);
            }
            else
            {
                uiPauseScript.pausePanel.SetActive(false);
                uiPauseScript.backToMenuPanel.SetActive(true);
            }
        }

        private void EnablePauseRestartPanel()
        {
            if (uiPauseScript.restartPanel.activeSelf)
            {
                uiPauseScript.pausePanel.SetActive(true);
                uiPauseScript.restartPanel.SetActive(false);
            }
            else
            {
                uiPauseScript.pausePanel.SetActive(false);
                uiPauseScript.restartPanel.SetActive(true);
            }
        }

        private void EnableConfigPause()
        {
            if (uiConfigInstance.activeSelf)
            {
                uiPauseScript.pausePanel.SetActive(true);
                uiConfigInstance.SetActive(false);
            }
            else
            {
                uiConfigScript.menuButton.onClick.RemoveAllListeners();
                uiConfigScript.menuButton.onClick.AddListener(() => EnableConfigPause());
                uiConfigScript.deleteButton.gameObject.SetActive(false);

                uiPauseScript.pausePanel.SetActive(false);
                uiConfigInstance.SetActive(true);
            }
        }

        private void CloseSkinMenu()
        {
            EnableSkinMenu();
            PlayerManager.RefreshData();
        }

        private void BackToMenu()
        {
            EnablePauseMenuPanel();
            GameManager.Menu();
        }

        private void RestartGame()
        {
            EnablePauseRestartPanel();
            GameManager.Menu();
            GameManager.PlayGame();
        }

        private void ReplayGame()
        {
            GameManager.Menu();
            GameManager.PlayGame();
        }

        private void DeleteData()
        {
            GameManager.SaveNewData(false);
            EnableConfigConfirmMenu();
            EnableConfigMenu();
            PlayerManager.RefreshData();
        }
        private IEnumerator BeginLevelCountDown()
        {
            uiTransition = true;
            yield return new WaitUntil(() => !CameraManager.OnGoingCameraTransition);
            uiGameScript.countDownText.gameObject.SetActive(true);
            uiGameScript.scoreText.gameObject.SetActive(false);
            float countDown = 3f;
            float timeElapsed = 0f;

            while (timeElapsed < countDown)
            {
                yield return new WaitUntil(() => !GameManager.isPaused);
                uiGameScript.countDownText.text = Mathf.Ceil(countDown - timeElapsed).ToString();
                timeElapsed += Time.deltaTime;
            }
            uiGameScript.countDownText.gameObject.SetActive(false);
            uiGameScript.scoreText.gameObject.SetActive(true);
            uiGameScript.countDownText.text = countDown.ToString();
            uiTransition = false;
            yield return null;
        }

        public IEnumerator SkinUnlocked()
        {
            uiGameScript.unlockText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            uiGameScript.unlockText.gameObject.SetActive(false);
            yield return null;
        }
    }
}