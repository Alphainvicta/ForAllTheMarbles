using UnityEngine;
using System.Collections.Generic;
using Managers;
using UnityEngine.SceneManagement;
using System.Collections;

public class DecorPool : MonoBehaviour
{
    public static DecorPool Instance { get; private set; }

    [Header("Basic Settings")]
    public GameObject decorPrefab;
    public int initialPoolSize = 20;
    public int preSpawnCount = 3;
    public float spawnRate = 1.5f;

    [Header("Movement")]
    public float decorSpeed = 8f;
    public float spawnZDistance = 50f;
    public float destroyZPosition = -15f;

    [Header("Exact Position")]
    public float fixedXPosition = 0f;
    public float fixedYPosition = 0f;

    [Header("Scale")]
    public float fixedScale = 1f;

    [Header("Delayed Destruction")]
    public float destructionDelay = 1.5f;

    private Queue<GameObject> decorPool = new Queue<GameObject>();
    private float nextSpawnTime;
    private bool isPaused = false;
    private bool gameRunning = false;

    private List<GameObject> preSpawnedDecor = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
            SubscribeToGameEvents();
            DontDestroyOnLoad(gameObject);
            PreSpawnDecorElements(preSpawnCount, true); // Pre-spawn for menu
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SubscribeToGameEvents()
    {
        GameManager.GameStart += OnGameStart;
        GameManager.GamePaused += OnGamePaused;
        GameManager.GameUnpaused += OnGameUnpaused;
        GameManager.GameEnd += OnGameEnd;
        GameManager.MenuGame += OnMenuGame;
    }

    private void UnsubscribeFromGameEvents()
    {
        GameManager.GameStart -= OnGameStart;
        GameManager.GamePaused -= OnGamePaused;
        GameManager.GameUnpaused -= OnGameUnpaused;
        GameManager.GameEnd -= OnGameEnd;
        GameManager.MenuGame -= OnMenuGame;
    }

    private void OnDestroy()
    {
        UnsubscribeFromGameEvents();
    }

    private void OnGameStart()
    {
        isPaused = true;
        gameRunning = true;
        nextSpawnTime = Time.time + spawnRate;
        StartCoroutine(DelayedStart(3f));
    }

    private IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        isPaused = false;
        PauseAllDecorElements(false);
    }

    private void OnGamePaused()
    {
        isPaused = true;
        PauseAllDecorElements(true);
    }

    private void OnGameUnpaused()
    {
        isPaused = false;
        PauseAllDecorElements(false);
        nextSpawnTime = Time.time + spawnRate;
    }

    private void OnGameEnd()
    {
        isPaused = true;  // Pausar movimiento
        gameRunning = true; // Seguir visibles
        PauseAllDecorElements(true); // Solo pausar movimiento
    }

    private void OnMenuGame()
    {
        isPaused = true;
        gameRunning = true;
        ResetDecor();
        PreSpawnDecorElements(preSpawnCount, true);
        PauseAllDecorElements(true);
    }

    private void ResetDecor()
    {
        foreach (var decor in preSpawnedDecor)
        {
            if (decor != null)
                ReturnDecorToPool(decor);
        }
        preSpawnedDecor.Clear();

        DecorElement[] allDecor = FindObjectsByType<DecorElement>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var decor in allDecor)
        {
            if (decor != null && decor.gameObject.activeInHierarchy)
            {
                ReturnDecorToPool(decor.gameObject);
            }
        }
    }

    private void Update()
    {
        if (!gameRunning || isPaused) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnDecorElement();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            AddNewDecorToPool();
        }
    }

    private void PreSpawnDecorElements(int count, bool pauseOnSpawn)
    {
        float spacing = 5f;
        for (int i = 0; i < count; i++)
        {
            GameObject decor = GetDecorElement();
            if (decor == null) continue;

            float zOffset = spawnZDistance - (spacing * i);
            decor.transform.position = new Vector3(fixedXPosition, fixedYPosition, zOffset);
            decor.transform.localScale = Vector3.one * fixedScale;
            decor.transform.rotation = Quaternion.identity;

            DecorElement element = decor.GetComponent<DecorElement>();
            if (element != null)
            {
                element.ResetElement();
                element.SetPaused(pauseOnSpawn);
            }

            decor.SetActive(true);
            preSpawnedDecor.Add(decor);
        }
    }

    private void AddNewDecorToPool()
    {
        if (decorPrefab == null)
        {
            Debug.LogError("DecorPrefab is not assigned in DecorPool");
            return;
        }

        GameObject decor = Instantiate(decorPrefab);
        decor.SetActive(false);

        DecorElement decorElement = decor.GetComponent<DecorElement>();
        if (decorElement == null)
        {
            decorElement = decor.AddComponent<DecorElement>();
        }

        decorElement.Initialize(this, decorSpeed, destructionDelay, destroyZPosition);
        decorPool.Enqueue(decor);
    }

    private void PauseAllDecorElements(bool pause)
    {
        DecorElement[] elements = FindObjectsByType<DecorElement>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (DecorElement element in elements)
        {
            if (element != null && element.gameObject.activeInHierarchy)
            {
                element.SetPaused(pause);
            }
        }
    }

    public GameObject GetDecorElement()
    {
        if (decorPool.Count == 0)
        {
            AddNewDecorToPool();
            Debug.LogWarning("Pool expanded. Consider increasing initialPoolSize");
        }

        return decorPool.Count > 0 ? decorPool.Dequeue() : null;
    }

    public void SpawnDecorElement()
    {
        GameObject decor = GetDecorElement();
        if (decor == null) return;

        decor.transform.position = new Vector3(
            fixedXPosition,
            fixedYPosition,
            spawnZDistance
        );

        decor.transform.localScale = Vector3.one * fixedScale;
        decor.transform.rotation = Quaternion.identity;

        DecorElement element = decor.GetComponent<DecorElement>();
        if (element != null)
        {
            element.ResetElement();
            element.SetPaused(false);
        }

        decor.SetActive(true);
    }

    public void ReturnDecorToPool(GameObject decorElement)
    {
        if (decorElement == null) return;

        DecorElement element = decorElement.GetComponent<DecorElement>();
        if (element != null)
        {
            element.ResetElement();
        }

        decorElement.SetActive(false);
        decorPool.Enqueue(decorElement);
    }

    public void UpdateFixedPosition(float newX, float newY)
    {
        fixedXPosition = newX;
        fixedYPosition = newY;
    }

    public void UpdateFixedScale(float newScale)
    {
        fixedScale = newScale;
    }

    public void UpdateDestructionDelay(float newDelay)
    {
        destructionDelay = newDelay;
        foreach (GameObject decor in decorPool)
        {
            if (decor != null)
            {
                DecorElement element = decor.GetComponent<DecorElement>();
                if (element != null)
                {
                    element.SetDestructionDelay(newDelay);
                }
            }
        }
    }
}
