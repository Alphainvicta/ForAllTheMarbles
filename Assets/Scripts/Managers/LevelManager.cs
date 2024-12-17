using UnityEngine;
using System.Collections;
using System.IO;
using Unity.VisualScripting;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public Level[] levelList;
        public Transform levelLocation;
        public GameObject poolLevel;
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
                    foreach (Level level in levelList)
                    {
                        StartCoroutine(SetObstacles(level));
                    }
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }

        private IEnumerator SetObstacles(Level level)
        {
            for (int i = 0; i < level.obstacleCount; i++)
            {
                GameObject randomObstacle = level.obstacleList[Random.Range(0, level.obstacleList.Length)];

                GameObject currentObstacle = LevelPool(randomObstacle);
                yield return new WaitUntil(() => currentObstacle.GetComponent<Obstacle_1>().obstacleEndPosition != null);
                levelLocation = currentObstacle.GetComponent<Obstacle_1>().obstacleEndPosition;
            }
            yield return null;
        }

        private GameObject LevelPool(GameObject wantedObject)
        {
            Transform[] obstacleList = poolLevel.GetComponentsInChildren<Transform>();
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