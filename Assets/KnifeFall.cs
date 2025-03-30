using UnityEngine;

public class KnifeFall : MonoBehaviour
{
    public float torqueForce = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("El objeto no tiene un Rigidbody.");
            return;
        }

        rb.AddTorque(Vector3.right * torqueForce, ForceMode.Impulse);
    }
}
