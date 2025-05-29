using UnityEngine;
using Managers;
using System.Collections;

public class FondoManager : MonoBehaviour, IPausable
{
    [Header("Configuración del fondo")]
    public GameObject fondoPrefab;
    public int cantidad = 5;
    public float velocidad = 2f;
    public float distanciaEntreFondos = 10f;
    public float zLimite = -20f;

    private GameObject[] fondos;
    private bool activo = false;

    private Coroutine delayCoroutine;

    void Awake()
    {
        fondos = new GameObject[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            Vector3 pos = new Vector3(0, 0, i * distanciaEntreFondos);
            fondos[i] = Instantiate(fondoPrefab, pos, Quaternion.identity, transform);
        }
    }

    void OnEnable()
    {
        GameManager.GameStart += IniciarConDelay;
        GameManager.GamePaused += Desactivar;
        GameManager.GameUnpaused += Activar;
        GameManager.GameEnd += Desactivar;
        GameManager.MenuGame += Desactivar;
        GameManager.StoreGame += Desactivar;

        if (GameManager.Instance != null)
            GameManager.Instance.RegisterPausable(this);
    }

    void OnDisable()
    {
        GameManager.GameStart -= IniciarConDelay;
        GameManager.GamePaused -= Desactivar;
        GameManager.GameUnpaused -= Activar;
        GameManager.GameEnd -= Desactivar;
        GameManager.MenuGame -= Desactivar;
        GameManager.StoreGame -= Desactivar;

        if (GameManager.Instance != null)
            GameManager.Instance.UnregisterPausable(this);
    }

    void Update()
    {
        if (!activo || GameManager.isPaused)
            return;

        for (int i = 0; i < fondos.Length; i++)
        {
            fondos[i].transform.position += Vector3.back * velocidad * Time.deltaTime;

            if (fondos[i].transform.position.z <= zLimite)
            {
                float zMax = ObtenerMaxZ();
                fondos[i].transform.position = new Vector3(
                    fondos[i].transform.position.x,
                    fondos[i].transform.position.y,
                    zMax + distanciaEntreFondos
                );
            }
        }
    }

    float ObtenerMaxZ()
    {
        float max = float.MinValue;
        foreach (GameObject fondo in fondos)
        {
            if (fondo.transform.position.z > max)
                max = fondo.transform.position.z;
        }
        return max;
    }

    void Activar()
    {
        activo = true;
    }

    void Desactivar()
    {
        activo = false;
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }
    }

    public void OnPause()
    {
        Desactivar();
    }

    public void OnResume()
    {
        Activar();
    }

    // Nueva función que inicia con delay
    void IniciarConDelay()
    {
        if (delayCoroutine != null)
            StopCoroutine(delayCoroutine);

        delayCoroutine = StartCoroutine(DelayStartCoroutine());
    }

    IEnumerator DelayStartCoroutine()
    {
        activo = false;
        yield return new WaitForSeconds(3f);
        activo = true;
        delayCoroutine = null;
    }
}
