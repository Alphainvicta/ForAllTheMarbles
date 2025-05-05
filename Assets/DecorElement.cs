using UnityEngine;
using System.Collections;
using System;

public class DecorElement : MonoBehaviour
{
    private DecorPool poolManager;
    private float speed;
    private float destroyZPosition;
    private float destructionDelay;
    private bool isCountingDown = false;
    private bool isPaused = false;
    private Vector3 pausedPosition;
    private Coroutine returnRoutine;

    public void Initialize(DecorPool manager, float speed, float delay, float destroyZ)
    {
        poolManager = manager;
        this.speed = speed;
        destructionDelay = delay;
        destroyZPosition = destroyZ;
    }

    private void OnDisable()
    {
        if (returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }
        isCountingDown = false;
    }

    private void Update()
    {
        if (isPaused || isCountingDown) return;

        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        if (transform.position.z < destroyZPosition && returnRoutine == null)
        {
            returnRoutine = StartCoroutine(DelayedReturnToPool());
        }
    }

    private IEnumerator DelayedReturnToPool()
    {
        if (poolManager == null) yield break;
        
        isCountingDown = true;
        SetRenderersEnabled(false);
        
        float timer = 0f;
        while (timer < destructionDelay)
        {
            if (!isPaused)
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }

        if (poolManager != null)
        {
            poolManager.ReturnDecorToPool(gameObject);
        }
        returnRoutine = null;
    }

    private void SetRenderersEnabled(bool enabled)
    {
        foreach(var renderer in GetComponentsInChildren<Renderer>())
        {
            if (renderer != null)
            {
                renderer.enabled = enabled;
            }
        }
    }

    public void SetPaused(bool paused)
    {
        if (isPaused == paused) return;

        isPaused = paused;
        if (paused)
        {
            pausedPosition = transform.position;
        }
        else
        {
            transform.position = pausedPosition;
        }
    }

    public void ResetElement()
    {
        if (returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }
        isCountingDown = false;
        isPaused = false;
        SetRenderersEnabled(true);
    }

    internal void SetDestructionDelay(float newDelay)
    {
        throw new NotImplementedException();
    }
}