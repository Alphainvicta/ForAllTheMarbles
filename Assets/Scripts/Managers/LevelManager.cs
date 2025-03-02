using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject levelPool;
        [SerializeField] private Level[] levelList;
        [SerializeField] private float levelSpeed;
        [SerializeField] private GameObject levelOrigin;
        [SerializeField] private GameObject levelPosition;
        private float disablePosition = -20f;
        private float generatePosition = 50f;
        private float directorChance;
        private bool canPlace = true;
        private bool readyToPlace = false;
        private GameObject levelPoolInstance;
        private static GameObject disablePool;
        private GameObject levelPositionInstance;
        private bool looped = false;
        private bool gameflag;
        private List<Coroutine> activeCoroutines = new();

        private void Start()
        {
            if (levelPoolInstance == null)
            {
                levelPoolInstance = Instantiate(levelPool, Vector3.zero, Quaternion.identity);
            }
            if (disablePool == null)
            {
                disablePool = new GameObject("DisablePool");
            }
            if (levelPositionInstance == null)
            {
                levelPositionInstance = Instantiate(levelPosition, levelOrigin.transform.position, Quaternion.identity);
                ObstacleGenerator(levelList[0].obstacles[0], levelList[0].levelFloor);
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
            StopAllActiveCoroutines();
            gameflag = false;
            levelPositionInstance.transform.position = levelOrigin.transform.position;

            List<Transform> children = new List<Transform>();
            foreach (Transform child in levelPoolInstance.transform)
            {
                children.Add(child);
            }

            foreach (Transform child in children)
            {
                child.SetParent(disablePool.transform);
                child.gameObject.SetActive(false);
            }

            CallFromPool(levelList[0].obstacles[0]);
        }

        private void OnGameStart()
        {
            StopAllActiveCoroutines();
            StartCoroutine(StartLevelAfterDelay());
        }

        private IEnumerator StartLevelAfterDelay()
        {
            Transform levelStart = levelPoolInstance.transform.Find("LevelStart");
            StartCoroutine(TransformObstacle(levelStart));
            StartCoroutine(TransformLevelPosition(levelPositionInstance.transform));
            StartCoroutine(LevelDirector());

            yield return new WaitUntil(() => !UiManager.LevelCountDownTransition);
            gameflag = true;
        }

        private void OnGamePaused()
        {
            Debug.Log("LevelManager: Game Paused");
        }

        private void OnGameUnpaused()
        {
            Debug.Log("LevelManager: Game Unpaused");
        }

        private void OnGameEnd()
        {
            StopAllActiveCoroutines();
        }

        private void OnStoreGame()
        {
            StopAllActiveCoroutines();
        }

        private int SetRandomPlacement(Obstacle.ObstacleWidth obstacleWidth, Obstacle.ObstaclePosition obstaclePosition)
        {
            switch (obstacleWidth)
            {
                case Obstacle.ObstacleWidth.Small:
                    return obstaclePosition switch
                    {
                        Obstacle.ObstaclePosition.Edges => Random.Range(0, 2) * 2,
                        Obstacle.ObstaclePosition.Center => 1,
                        _ => Random.Range(0, 3),
                    };

                case Obstacle.ObstacleWidth.Medium:
                    return Random.Range(0, 2);

                default:
                    return 0;
            }
        }

        private Transform ObstacleGenerator(Obstacle obstacle, GameObject levelFloor)
        {
            levelPositionInstance.transform.parent = null;
            int randomPlacement = SetRandomPlacement(obstacle.obstacleWidth, obstacle.obstaclePosition);
            GameObject newFloor = null;

            GameObject obstacleParent = new(obstacle.name);
            obstacleParent.transform.SetParent(levelPoolInstance.transform);
            obstacleParent.transform.position = levelPositionInstance.transform.position;

            GameObject newObstacle = Instantiate(obstacle.obstaclePrefab,
                obstacleParent.transform.position + new Vector3(randomPlacement * 2f, 0f, 0f) + obstacle.obstaclePrefab.transform.localPosition,
                Quaternion.identity, obstacleParent.transform);
            newObstacle.name = obstacle.obstaclePrefab.name;

            levelPositionInstance.transform.SetParent(obstacleParent.transform);

            for (int i = 0; i < obstacle.ObstacleLength; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (obstacle.needFloor)
                    {
                        newFloor = Instantiate(levelFloor, levelPositionInstance.transform.position + new Vector3(1f + (2 * j), 0f, 1f) + levelFloor.transform.localPosition, Quaternion.identity, obstacleParent.transform);
                    }
                    else if (!obstacle.needFloor && j != randomPlacement)
                    {
                        newFloor = Instantiate(levelFloor, levelPositionInstance.transform.position + new Vector3(1f + (2 * j), 0f, 1f) + levelFloor.transform.localPosition, Quaternion.identity, obstacleParent.transform);
                    }
                }
                levelPositionInstance.transform.position += new Vector3(0f, 0f, 2f);
            }
            return obstacleParent.transform;
        }

        private bool CallFromPool(Obstacle wantedObstacle)
        {
            levelPositionInstance.transform.parent = null;
            Transform[] poolObstacleList = disablePool.GetComponentsInChildren<Transform>(true);
            if (poolObstacleList.Length > 0)
            {
                foreach (Transform obstacleParent in poolObstacleList)
                {
                    if (obstacleParent.name.Contains(wantedObstacle.name) && !obstacleParent.gameObject.activeSelf)
                    {
                        obstacleParent.gameObject.SetActive(true);
                        obstacleParent.SetParent(levelPoolInstance.transform);
                        obstacleParent.position = levelPositionInstance.transform.position;

                        if (wantedObstacle.needFloor)
                        {
                            int randomPosition = SetRandomPlacement(wantedObstacle.obstacleWidth, wantedObstacle.obstaclePosition);
                            Transform obstacle = obstacleParent.transform.Find(wantedObstacle.obstaclePrefab.name);
                            obstacle.localPosition = new Vector3(randomPosition * 2f, 0f, 0f) + wantedObstacle.obstaclePrefab.transform.localPosition;
                        }

                        levelPositionInstance.transform.position += new Vector3(0f, 0f, 2f * wantedObstacle.ObstacleLength);
                        levelPositionInstance.transform.SetParent(obstacleParent.transform);
                        StartCoroutine(TransformObstacle(obstacleParent.transform));
                        return true;
                    }
                }
            }
            return false;
        }

        private IEnumerator LevelDirector()
        {
            directorChance = 25f;
            StartCoroutine(ChanceIncrement());
            while (true)
            {
                if (!looped)
                {
                    for (int i = 1; i < levelList.Length; i++)
                    {
                        for (int j = 0; j < levelList[i].baseLevelLength; j++)
                        {
                            yield return new WaitUntil(() => canPlace);
                            canPlace = false;

                            rollChances();

                            if (readyToPlace)
                            {
                                readyToPlace = false;
                                int randomObstacle = UnityEngine.Random.Range(0, levelList[i].obstacles.Length);
                                if (CallFromPool(levelList[i].obstacles[randomObstacle]))
                                    continue;
                                StartCoroutine(
                                    TransformObstacle(
                                    ObstacleGenerator(levelList[i].obstacles[randomObstacle], levelList[i].levelFloor)
                                    )
                                    );
                            }
                            else
                            {
                                j--;
                                if (CallFromPool(levelList[0].obstacles[1]))
                                    continue;
                                StartCoroutine(
                                    TransformObstacle(
                                    ObstacleGenerator(levelList[0].obstacles[1], levelList[i].levelFloor)
                                    )
                                    );
                            }
                        }
                    }
                }

                else
                {
                    yield return new WaitUntil(() => canPlace);
                    canPlace = false;

                    rollChances();

                    if (readyToPlace)
                    {
                        int randomLevel = UnityEngine.Random.Range(1, levelList.Length);
                        int randomObstacle = UnityEngine.Random.Range(0, levelList[randomLevel].obstacles.Length);
                        if (CallFromPool(levelList[randomLevel].obstacles[randomObstacle]))
                            continue;
                        StartCoroutine(
                            TransformObstacle(
                            ObstacleGenerator(levelList[randomLevel].obstacles[randomObstacle], levelList[randomLevel].levelFloor)
                            )
                            );
                    }
                    else
                    {
                        int randomLevel = UnityEngine.Random.Range(1, levelList.Length);
                        if (CallFromPool(levelList[0].obstacles[1]))
                            continue;
                        StartCoroutine(
                            TransformObstacle(
                            ObstacleGenerator(levelList[0].obstacles[1], levelList[randomLevel].levelFloor)
                            )
                            );
                    }
                }

                yield return null;
            }
        }

        private void rollChances()
        {
            if (directorChance >= UnityEngine.Random.Range(1, 101))
                readyToPlace = true;
            else
                readyToPlace = false;
        }

        private IEnumerator ChanceIncrement()
        {
            while (directorChance < 101)
            {
                yield return new WaitForSeconds(30f);
                directorChance++;
            }
        }

        private IEnumerator TransformObstacle(Transform obstacle)
        {
            while (obstacle.position.z > disablePosition)
            {
                yield return new WaitUntil(() => !GameManager.isPaused && gameflag);
                obstacle.position += Vector3.back * levelSpeed * Time.deltaTime;
                yield return null;
            }
            obstacle.SetParent(disablePool.transform);
            obstacle.gameObject.SetActive(false);
            yield return null;
        }

        private IEnumerator TransformLevelPosition(Transform levelPositionInstance)
        {
            while (true)
            {
                if (levelPositionInstance.position.z < generatePosition)
                    canPlace = true;
                yield return null;
            }
        }
        private void StopAllActiveCoroutines()
        {
            foreach (var coroutine in activeCoroutines)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
            activeCoroutines.Clear();
        }
        private new Coroutine StartCoroutine(IEnumerator routine)
        {
            Coroutine coroutine = base.StartCoroutine(routine);
            activeCoroutines.Add(coroutine);
            return coroutine;
        }
    }
}