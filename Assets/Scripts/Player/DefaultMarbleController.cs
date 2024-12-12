using UnityEngine;

public class DefaultMarbleController : MonoBehaviour
{
    private Rigidbody rigidbody;
    private GameObject cameraRig;

    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        cameraRig = GameObject.Find("CameraRig");
    }

    private void Update()
    {
        rigidbody.AddForce(new Vector3(0f, 0f, 1f));
        CameraFollow();
    }

    private void CameraFollow()
    {
        cameraRig.transform.position = new Vector3(transform.position.x, cameraRig.transform.position.y, transform.position.z);
    }
}
