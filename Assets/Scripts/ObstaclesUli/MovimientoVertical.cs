using UnityEngine;

public class MovimientoVertical : MonoBehaviour
{
    [Header("Configuración")]
    public float distancia = 2f; 
    public float velocidad = 2f; 

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        float desplazamiento = Mathf.Sin(Time.time * velocidad) * distancia;
        transform.position = posicionInicial + Vector3.up * desplazamiento;
    }
}