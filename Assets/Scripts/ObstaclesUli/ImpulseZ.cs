using UnityEngine;

public class ImpulseZ : MonoBehaviour
{
    public float impulseForce = 10f; 
    private Rigidbody rb;
    private bool toggleImpulse = true; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            enabled = false;
        }
        else
        {
            InvokeRepeating("ApplyImpulse", 0f, 3f);
        }
    }

    void ApplyImpulse()
    {
        Vector3 impulse = new Vector3(0, 0, toggleImpulse ? impulseForce : -impulseForce);
        rb.AddForce(impulse, ForceMode.Impulse);
        toggleImpulse = !toggleImpulse;
    }
}