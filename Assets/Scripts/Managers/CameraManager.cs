using UnityEngine;
using System.Collections;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainCameraPrefab;
        [SerializeField] private AnimationCurve cameraTransitionCurve;
        public static GameObject mainCameraInstance;
        public static readonly float cameraGameStartTransitionDuration = 1.5f;
        private Coroutine cameraTransitionCoroutine;

        private void Start()
        {
            if (mainCameraInstance == null)
            {
                mainCameraInstance = Instantiate(mainCameraPrefab, new Vector3(3.5f, 1f, 0f), Quaternion.Euler(0f, -90f, 0f));
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
            if (cameraTransitionCoroutine != null)
            {
                StopCoroutine(cameraTransitionCoroutine);
            }
            cameraTransitionCoroutine = StartCoroutine(CameraTransition(new Vector3(3.5f, 1f, 0f), Quaternion.Euler(0f, -90f, 0f), 3f));
        }

        private void OnGameStart()
        {
            if (cameraTransitionCoroutine != null)
            {
                StopCoroutine(cameraTransitionCoroutine);
            }
            cameraTransitionCoroutine = StartCoroutine(CameraTransition(new Vector3(0f, 5f, -10f), Quaternion.Euler(5f, 0f, 0f), cameraGameStartTransitionDuration));
        }

        private void OnGamePaused()
        {
            Debug.Log("CameraManager: Game Paused");
        }

        private void OnGameUnpaused()
        {
            Debug.Log("CameraManager: Game Unpaused");
        }

        private void OnGameEnd()
        {
            Debug.Log("CameraManager: Game End");
        }

        private void OnStoreGame()
        {
            Debug.Log("CameraManager: Store Game");
        }

        public IEnumerator CameraTransition(Vector3 goalPosition, Quaternion goalRotation, float duration)
        {
            mainCameraInstance.transform.GetPositionAndRotation(out Vector3 currentPosition, out Quaternion currentRotation);
            float timeElapsed = 0f;
            while (timeElapsed < duration)
            {
                float t = cameraTransitionCurve.Evaluate(timeElapsed / duration);
                mainCameraInstance.transform.SetPositionAndRotation(Vector3.Lerp(currentPosition, goalPosition, t), Quaternion.Lerp(currentRotation, goalRotation, t));
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            mainCameraInstance.transform.SetPositionAndRotation(goalPosition, goalRotation);
            yield return null;
        }
    }
}