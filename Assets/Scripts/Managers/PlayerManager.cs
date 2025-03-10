using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        public PlayerMarbles playerMarbles;
        public static GameObject playerInstance;
        private int marbleIndex;
        private Vector3 savedVelocity;
        private Vector3 savedAngularVelocity;
        public static bool isObstacleHitted;

        private void Start()
        {
            marbleIndex = playerMarbles.LoadPlayerMarbles();
            if (playerInstance == null)
            {
                playerInstance = Instantiate(playerMarbles.marbles[marbleIndex].marblePrefab, Vector3.zero, Quaternion.identity);
                UpdateMarbleMaterial();
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
            isObstacleHitted = false;
            playerInstance.GetComponent<PlayerInput>().enabled = false;
            playerInstance.GetComponent<Collider>().enabled = true;
            playerInstance.GetComponent<Rigidbody>().isKinematic = false;

            RestoreMarbleValues();
        }

        private void OnGameStart()
        {
            isObstacleHitted = false;
            playerInstance.GetComponent<Collider>().enabled = true;
            playerInstance.GetComponent<Rigidbody>().isKinematic = false;
            StartCoroutine(EnablePlayerInputAfterDelay());
        }

        private IEnumerator EnablePlayerInputAfterDelay()
        {
            yield return new WaitUntil(() => !UiManager.uiTransition);
            playerInstance.GetComponent<PlayerInput>().enabled = true;
        }

        private void OnGamePaused()
        {
            playerInstance.GetComponent<PlayerInput>().enabled = false;

            playerInstance.GetComponent<Collider>().enabled = false;
            savedVelocity = playerInstance.GetComponent<Rigidbody>().linearVelocity;
            savedAngularVelocity = playerInstance.GetComponent<Rigidbody>().angularVelocity;
            playerInstance.GetComponent<Rigidbody>().isKinematic = true;
        }

        private void OnGameUnpaused()
        {
            playerInstance.GetComponent<PlayerInput>().enabled = true;

            playerInstance.GetComponent<Collider>().enabled = true;
            playerInstance.GetComponent<Rigidbody>().isKinematic = false;
            playerInstance.GetComponent<Rigidbody>().linearVelocity = savedVelocity;
            playerInstance.GetComponent<Rigidbody>().angularVelocity = savedAngularVelocity;
        }

        private void OnGameEnd()
        {
            isObstacleHitted = true;
            playerInstance.GetComponent<PlayerInput>().enabled = false;
            playerInstance.GetComponent<Collider>().enabled = false;
            playerInstance.GetComponent<Rigidbody>().isKinematic = true;

        }

        private void OnStoreGame()
        {
            isObstacleHitted = false;
            playerInstance.GetComponent<PlayerInput>().enabled = false;
            playerInstance.GetComponent<Collider>().enabled = true;

            RestoreMarbleValues();
        }

        public void NextPlayerMarble(bool right)
        {
            if (right)
            {
                if (marbleIndex < playerMarbles.marbles.Count - 1)
                {
                    marbleIndex++;
                }
                else
                {
                    marbleIndex = 0;
                }
            }
            else
            {
                if (marbleIndex > 0)
                {
                    marbleIndex--;
                }
                else
                {
                    marbleIndex = playerMarbles.marbles.Count - 1;
                }
            }

            if (playerInstance != null)
            {
                Destroy(playerInstance);
                playerInstance = Instantiate(playerMarbles.marbles[marbleIndex].marblePrefab, Vector3.zero, Quaternion.identity);
                UpdateMarbleMaterial();
            }

            playerMarbles.SavePlayerMarbles(marbleIndex);
            playerInstance.GetComponent<PlayerInput>().enabled = false;
        }

        private void UpdateMarbleMaterial()
        {
            if (!playerMarbles.marbles[marbleIndex].isUnlocked)
            {
                Renderer renderer = playerInstance.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.black;
                }
            }
            else
            {
                Renderer renderer = playerInstance.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.white;
                }
            }
        }

        private void RestoreMarbleValues()
        {
            playerInstance.GetComponent<Rigidbody>().isKinematic = false;
            playerInstance.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            playerInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            playerInstance.transform.position = Vector3.zero;
        }
    }
}