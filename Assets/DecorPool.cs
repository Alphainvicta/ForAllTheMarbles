using UnityEngine;
using System.Collections.Generic;
using Managers;
using UnityEngine.SceneManagement;

public class DecorPool : MonoBehaviour
{
    public static DecorPool Instance { get; private set; }

    [Header("Basic Settings")]
    public GameObject decorPrefab;
    public int initialPoolSize = 20;
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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
            SubscribeToGameEvents();
            DontDestroyOnLoad(gameObject);
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
        gameRunning = true;
        isPaused = false;
        nextSpawnTime = Time.time + spawnRate;
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
        gameRunning = false;
        ReturnAllToPool();
    }

    private void OnMenuGame()
    {
        gameRunning = false;
        ReturnAllToPool();
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
        foreach (GameObject decor in decorPool)
        {
            if (decor != null && decor.activeInHierarchy)
            {
                DecorElement element = decor.GetComponent<DecorElement>();
                if (element != null)
                {
                    element.SetPaused(pause);
                }
            }
        }
    }

    private void ReturnAllToPool()
    {
        DecorElement[] activeElements = FindObjectsByType<DecorElement>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        foreach (DecorElement element in activeElements)
        {
            if (element != null && element.gameObject.activeInHierarchy)
            {
                ReturnDecorToPool(element.gameObject);
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