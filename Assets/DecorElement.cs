using UnityEngine;

public class DecorElement : MonoBehaviour
{
    private float speed;
    private float destroyZ;
    private float delay;
    private bool paused = false;
    private bool initialized = false;
    private DecorPool pool;

    public void Initialize(DecorPool pool, float speed, float delay, float destroyZ)
    {
        this.pool = pool;
        this.speed = speed;
        this.delay = delay;
        this.destroyZ = destroyZ;
        initialized = true;
    }

    public void ResetElement()
    {
        paused = false;
    }

    public void SetPaused(bool value)
    {
        paused = value;
    }

    public void SetDestructionDelay(float newDelay)
    {
        delay = newDelay;
    }

    private void Update()
    {
        if (!initialized || paused) return;

        transform.Translate(Vector3.back * speed * Time.deltaTime);

        if (transform.position.z < destroyZ)
        {
            pool.ReturnDecorToPool(gameObject);
        }
    }
}
