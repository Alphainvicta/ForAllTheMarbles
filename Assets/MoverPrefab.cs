using UnityEngine;
using Managers;  // Para acceder al GameManager

public class MoverPrefab : MonoBehaviour, IPausable
{
    private Rigidbody rb;  // Rigidbody para controlar el movimiento

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();  // Obtenemos el Rigidbody
    }

    // Pausar el movimiento del prefab (congelar rotación)
    public void OnPause()
    {
        if (rb != null)
        {
            // Congelamos la rotación en el eje X
            rb.freezeRotation = true;  // Congelamos la rotación en todos los ejes
        }
    }

    // Reanudar el movimiento del prefab (liberar rotación)
    public void OnResume()
    {
        if (rb != null)
        {
            // Liberamos la rotación en el eje X
            rb.freezeRotation = false;  // Liberamos la rotación para que pueda rotar de nuevo
        }
    }

    // Puedes agregar un método para asegurarte de que el prefab se registre con el GameManager
    private void OnEnable()
    {
        GameManager.Instance.RegisterPausable(this);  // Registramos el prefab
    }

    private void OnDisable()
    {
        GameManager.Instance.UnregisterPausable(this);  // Desregistramos el prefab
    }
}
