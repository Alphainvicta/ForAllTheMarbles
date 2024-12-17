using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public Level[] levelList;
        public Transform levelLocation;
        public GameObject poolLevel;
        public Transform cameraPosition;
        private List<Transform> activeObstacles = new();
        private bool isAllowed;
        private float disableRange = -30f;
        private float enableRange = 30f;
        private void OnEnable()
        {
            GameManager.GameStart += HandleGameStart;
            GameManager.GameEnd += HandleGameEnd;
            GameManager.GameMenu += HandleGameEnd;
        }

        private void OnDisable()
        {
            GameManager.GameStart -= HandleGameStart;
            GameManager.GameEnd -= HandleGameEnd;
            GameManager.GameMenu -= HandleGameEnd;
        }

        private void HandleGameStart() => GameLevel(1);
        private void HandleGameEnd() => GameLevel(2);

        private void GameLevel(int option)
        {
            switch (option)
            {
                case 1:
                    StartCoroutine(SetObstacles());
                    StartCoroutine(ObstacleManager());
                    break;
                case 2:
                    StopAllCoroutines();
                    break;
                default:
                    break;
            }
        }

        private IEnumerator SetObstacles()
        {
            foreach (Level level in levelList)
            {
                for (int i = 0; i < level.obstacleCount; i++)
                {
                    isAllowed = false;
                    activeObstacles.Add(levelLocation);
                    yield return new WaitUntil(() => isAllowed);
                    GameObject randomObstacle = level.obstacleList[Random.Range(0, level.obstacleList.Length)];

                    GameObject currentObstacle = LevelPool(randomObstacle);
                    yield return new WaitUntil(() => currentObstacle.GetComponent<ObstacleEndPosition>().obstacleEndPosition != null);
                    levelLocation = currentObstacle.GetComponent<ObstacleEndPosition>().obstacleEndPosition;
                }
            }
            yield return null;
        }

        private IEnumerator ObstacleManager()
        {
            while (true)
            {
                yield return new WaitUntil(() => activeObstacles.Count > 0);

                for (int i = 0; i < activeObstacles.Count; i++)
                {
                    if (i == activeObstacles.Count - 1 && !isAllowed)
                    {
                        if (levelLocation.position.z - cameraPosition.position.z < enableRange)
                        {
                            isAllowed = true;
                        }
                    }
                    else
                    {
                        if (activeObstacles[i].position.z - cameraPosition.position.z < disableRange)
                        {
                            if (activeObstacles[i].parent != null)
                            {
                                activeObstacles[i].parent.gameObject.SetActive(false);
                                activeObstacles.Remove(activeObstacles[i]);
                            }
                            else
                            {
                                activeObstacles[i].gameObject.SetActive(false);
                                activeObstacles.Remove(activeObstacles[i]);
                            }
                        }
                    }
                }
            }
        }

        private GameObject LevelPool(GameObject wantedObject)
        {
            Transform[] obstacleList = poolLevel.GetComponentsInChildren<Transform>(true);
            Vector3 obstaclePosition = new(levelLocation.position.x, levelLocation.position.y, levelLocation.position.z + (levelLocation.localScale.z / 2));

            foreach (Transform obstacle in obstacleList)
            {
                if (obstacle.name.Contains(wantedObject.name) && !obstacle.gameObject.activeSelf)
                {
                    obstacle.position = obstaclePosition;
                    obstacle.gameObject.SetActive(true);
                    return obstacle.gameObject;
                }
            }
            GameObject newObstacle = Instantiate(wantedObject, obstaclePosition, levelLocation.rotation, poolLevel.transform);
            return newObstacle;
        }
    }
}