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
        private bool canPlace = true;
        private bool readyToPlace = false;
        private GameObject levelPoolInstance;
        private static GameObject disablePool;
        private GameObject levelPositionInstance;
        private bool looped = false;
        private bool gameflag;
        public static bool levelEnding;
        private List<Coroutine> activeCoroutines = new();
        private Dictionary<string, List<Transform>> obstaclePool = new();

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
            levelEnding = false;
            levelPositionInstance.transform.position = levelOrigin.transform.position;

            List<Transform> children = new List<Transform>();
            foreach (Transform child in levelPoolInstance.transform)
            {
                children.Add(child);
            }

            foreach (Transform child in children)
            {
                foreach (Transform floorChild in child)
                {
                    if (floorChild.CompareTag("Ground"))
                    {
                        ReturnToDisablePool(child, floorChild.gameObject);
                        break;
                    }
                }
            }

            CallFromPool(levelList[0].obstacles[0], levelList[0].levelFloor);
        }

        private void OnGameStart()
        {
            levelEnding = false;

            StopAllActiveCoroutines();
            StartCoroutine(StartLevelAfterDelay());
        }

        private IEnumerator StartLevelAfterDelay()
        {
            Transform levelStart = levelPoolInstance.transform.Find(levelList[0].obstacles[0].name);
            StartCoroutine(TransformObstacle(levelStart, 0));
            StartCoroutine(TransformLevelPosition(levelPositionInstance.transform));
            StartCoroutine(LevelDirector());

            yield return new WaitUntil(() => !UiManager.uiTransition);
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
            StartCoroutine(GameEnd());
        }

        private void OnStoreGame()
        {
            levelEnding = false;
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

            string obstacleKey = levelFloor.name + obstacle.name;

            if (!obstaclePool.ContainsKey(obstacleKey))
            {
                obstaclePool[obstacleKey] = new List<Transform>();
            }

            return obstacleParent.transform;
        }

        private Transform CallFromPool(Obstacle wantedObstacle, GameObject levelFloor)
        {
            levelPositionInstance.transform.parent = null;
            string obstacleKey = levelFloor.name + wantedObstacle.name;
            if (obstaclePool.ContainsKey(obstacleKey) && obstaclePool[obstacleKey].Count > 0)
            {
                List<Transform> obstacleTransformList = obstaclePool[obstacleKey];

                Transform obstacleParent = obstacleTransformList[0];
                obstacleTransformList.RemoveAt(0);

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

                obstaclePool[obstacleKey] = obstacleTransformList;

                return obstacleParent.transform;
            }

            return null;
        }

        private IEnumerator LevelDirector()
        {
            PlayerManager playerManager = FindFirstObjectByType<PlayerManager>();
            looped = false;
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

                            int randomObstacle = Random.Range(0, levelList[i].obstacles.Length);

                            Transform pooledObstacle = CallFromPool(levelList[i].obstacles[randomObstacle], levelList[i].levelFloor);
                            if (pooledObstacle != null)
                            {
                                if (j == levelList[i].baseLevelLength - 1)
                                    StartCoroutine(TransformObstacle(pooledObstacle, i, true, playerManager));

                                else
                                    StartCoroutine(TransformObstacle(pooledObstacle, i));
                            }

                            else
                            {
                                if (j == levelList[i].baseLevelLength - 1)
                                    StartCoroutine(
                                        TransformObstacle(
                                        ObstacleGenerator(levelList[i].obstacles[randomObstacle], levelList[i].levelFloor), i, true, playerManager
                                        )
                                        );
                                else
                                    StartCoroutine(
                                        TransformObstacle(
                                        ObstacleGenerator(levelList[i].obstacles[randomObstacle], levelList[i].levelFloor), i
                                        )
                                        );
                            }
                        }
                    }

                    looped = true;
                }

                else
                {
                    yield return new WaitUntil(() => canPlace);
                    canPlace = false;

                    int randomLevel = Random.Range(1, levelList.Length);
                    int randomObstacle = Random.Range(0, levelList[randomLevel].obstacles.Length);

                    Transform pooledObstacle = CallFromPool(levelList[randomLevel].obstacles[randomObstacle], levelList[randomLevel].levelFloor);
                    if (pooledObstacle != null)
                        StartCoroutine(
                        TransformObstacle(pooledObstacle, randomLevel));
                    else
                        StartCoroutine(
                            TransformObstacle(
                            ObstacleGenerator(levelList[randomLevel].obstacles[randomObstacle], levelList[randomLevel].levelFloor), randomLevel
                            )
                            );
                }

                yield return null;
            }
        }

        private IEnumerator TransformObstacle(Transform obstacle, int i, bool isUnlockable = false, PlayerManager playerManager = null)
        {
            while (obstacle.position.z > disablePosition)
            {
                yield return new WaitUntil(() => !GameManager.isPaused && gameflag);
                obstacle.position += Vector3.back * levelSpeed * Time.deltaTime;

                if (isUnlockable)
                {
                    if (obstacle.position.z <= 0f)
                    {
                        if (i < playerManager.playerMarbles.marbles.Count)
                        {
                            if (!playerManager.playerMarbles.marbles[i].isUnlocked)
                            {
                                UiManager uiManager = FindFirstObjectByType<UiManager>();
                                StartCoroutine(uiManager.SkinUnlocked());
                                playerManager.playerMarbles.marbles[i].isUnlocked = true;
                                playerManager.playerMarbles.SavePlayerMarbles(playerManager.marbleIndex);
                            }
                        }
                        isUnlockable = false;
                    }
                }
                yield return null;
            }
            ReturnToDisablePool(obstacle, levelList[i].levelFloor);

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

        private IEnumerator TransformEndGameObstacles(Transform levelPosition)
        {
            readyToPlace = true;
            while (levelPosition.position.z < levelOrigin.transform.position.z)
            {
                yield return new WaitUntil(() => !GameManager.isPaused && gameflag);
                levelPosition.position += Vector3.forward * 30 * Time.deltaTime;
                yield return null;
            }
            levelPosition.position = levelOrigin.transform.position;
            readyToPlace = false;
            yield return null;
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

        private void ReturnToDisablePool(Transform obstacle, GameObject levelFloor)
        {
            string floorName = levelFloor.name.Replace("(Clone)", "");
            string obstacleKey = floorName + obstacle.name;
            obstacle.SetParent(disablePool.transform);
            obstacle.gameObject.SetActive(false);
            obstaclePool[obstacleKey].Add(obstacle);
        }

        private IEnumerator GameEnd()
        {
            levelEnding = true;
            yield return new WaitForSeconds(1f);

            levelPositionInstance.transform.position = levelPoolInstance.transform.GetChild(0).position - new Vector3(0f, 0f, 2f * levelList[0].obstacles[0].ObstacleLength);
            if (CallFromPool(levelList[0].obstacles[0], levelList[0].levelFloor) == null)
            {
                levelPositionInstance.transform.position = levelPoolInstance.transform.GetChild(0).position;
            }
            else
            {
                levelPositionInstance.transform.parent = null;
                levelPositionInstance.transform.position -= new Vector3(0f, 0f, 2f * levelList[0].obstacles[0].ObstacleLength);
            }

            MoveToNewParent(levelPoolInstance.transform, levelPositionInstance.transform);
            StartCoroutine(TransformEndGameObstacles(levelPositionInstance.transform));
            yield return new WaitUntil(() => !readyToPlace);
            MoveToNewParent(levelPositionInstance.transform, levelPoolInstance.transform);

            levelEnding = false;
            yield return null;
        }

        private void MoveToNewParent(Transform oldParent, Transform newParent)
        {
            while (oldParent.childCount > 0)
            {
                Transform child = oldParent.GetChild(0);
                child.SetParent(newParent);
            }
        }
    }
}