using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private GameObject uiMenuPrefab;
        [SerializeField] private GameObject uiGamePrefab;
        [SerializeField] private GameObject uiPausedPrefab;
        [SerializeField] private GameObject uiEndPrefab;
        [SerializeField] private GameObject uiStorePrefab;
        private GameObject uiMenuInstance;
        private GameObject uiGameInstance;
        private GameObject uiPausedInstance;
        private GameObject uiEndInstance;
        private GameObject uiStoreInstance;

        private Button menuRightButton;
        private Button menuLeftButton;
        private Button menuPlayButton;

        private Transform levelCountDownText;

        PlayerManager playerManager;

        private void Start()
        {
            if (uiMenuInstance == null)
            {
                uiMenuInstance = Instantiate(uiMenuPrefab, Vector3.zero, Quaternion.identity);

                playerManager = FindFirstObjectByType<PlayerManager>();

                AssignMenuButtons();
            }

            if (uiGameInstance == null)
            {
                uiGameInstance = Instantiate(uiGamePrefab, Vector3.zero, Quaternion.identity);
            }
            if (uiPausedInstance == null)
            {
                uiPausedInstance = Instantiate(uiPausedPrefab, Vector3.zero, Quaternion.identity);
            }
            if (uiEndInstance == null)
            {
                uiEndInstance = Instantiate(uiEndPrefab, Vector3.zero, Quaternion.identity);
            }
            if (uiStoreInstance == null)
            {
                uiStoreInstance = Instantiate(uiStorePrefab, Vector3.zero, Quaternion.identity);
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
            uiMenuInstance.SetActive(true);
            uiGameInstance.SetActive(false);
            uiPausedInstance.SetActive(false);
            uiEndInstance.SetActive(false);
            uiStoreInstance.SetActive(false);
        }

        private void OnGameStart()
        {
            uiMenuInstance.SetActive(false);
            uiGameInstance.SetActive(true);
            uiPausedInstance.SetActive(false);
            uiEndInstance.SetActive(false);
            uiStoreInstance.SetActive(false);

            StartCoroutine(BeginLevelCountDown());
        }

        private void OnGamePaused()
        {
            uiMenuInstance.SetActive(false);
            uiGameInstance.SetActive(false);
            uiPausedInstance.SetActive(true);
            uiEndInstance.SetActive(false);
            uiStoreInstance.SetActive(false);
        }

        private void OnGameUnpaused()
        {
            uiMenuInstance.SetActive(false);
            uiGameInstance.SetActive(true);
            uiPausedInstance.SetActive(false);
            uiEndInstance.SetActive(false);
            uiStoreInstance.SetActive(false);
        }

        private void OnGameEnd()
        {
            uiMenuInstance.SetActive(false);
            uiGameInstance.SetActive(false);
            uiPausedInstance.SetActive(false);
            uiEndInstance.SetActive(true);
            uiStoreInstance.SetActive(false);
        }

        private void OnStoreGame()
        {
            uiMenuInstance.SetActive(false);
            uiGameInstance.SetActive(false);
            uiPausedInstance.SetActive(false);
            uiEndInstance.SetActive(false);
            uiStoreInstance.SetActive(true);
        }

        private void AssignMenuButtons()
        {
            Transform canvasChildren = uiMenuInstance.transform.Find("MenuCanvas");
            if (canvasChildren != null)
            {
                menuRightButton = canvasChildren.Find("RightButton")?.GetComponent<Button>();
                menuLeftButton = canvasChildren.Find("LeftButton")?.GetComponent<Button>();
                menuPlayButton = canvasChildren.Find("PlayButton")?.GetComponent<Button>();

                if (menuRightButton != null)
                {
                    menuRightButton.onClick.AddListener(() => playerManager.NextPlayerMarble(true));
                }
                else
                {
                    Debug.LogError("RightButton not found in canvasChildren!");
                }

                if (menuLeftButton != null)
                {
                    menuLeftButton.onClick.AddListener(() => playerManager.NextPlayerMarble(false));
                }
                else
                {
                    Debug.LogError("LeftButton not found in canvasChildren!");
                }
                if (menuPlayButton != null)
                {
                    menuPlayButton.onClick.AddListener(() => GameManager.PlayGame());
                }
                else
                {
                    Debug.LogError("PlayButton not found in canvasChildren!");
                }
            }
            else
            {
                Debug.LogError("canvasChildren not found in uiMenuInstance!");
            }
        }

        private IEnumerator BeginLevelCountDown()
        {
            Transform canvasChildren = uiGameInstance.transform.Find("GameCanvas");
            if (canvasChildren != null)
                yield return new WaitForSeconds(CameraManager.cameraGameStartTransitionDuration);
            levelCountDownText = canvasChildren.Find("CountDown");
            levelCountDownText.gameObject.SetActive(true);
            float countDown = 3f;

            while (countDown > 0)
            {
                levelCountDownText.GetComponent<TextMeshProUGUI>().text = countDown.ToString();
                yield return new WaitForSeconds(1f);
                countDown--;
            }
            levelCountDownText.gameObject.SetActive(false);
            yield return null;
        }
    }
}