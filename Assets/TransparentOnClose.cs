using UnityEngine;

public class TransparentOnClose : MonoBehaviour
{
    public float fadeDistance = 5f;    
    public float minAlpha = 0.2f;      
    private Material mat;
    private Color originalColor;
    private Transform cameraTransform;

    void Start()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró una cámara principal en la escena.");
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            mat = new Material(renderer.material);
            renderer.material = mat;
            originalColor = mat.color;

            mat.SetFloat("_Mode", 3); 
            mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
        }
        else
        {
            Debug.LogWarning("No se encontró un Renderer en " + gameObject.name);
        }
    }

    void Update()
    {
        if (cameraTransform == null || mat == null)
            return;

        float distance = Vector3.Distance(transform.position, cameraTransform.position);
        float alpha = Mathf.Clamp01((distance - 1f) / fadeDistance);

        Color newColor = originalColor;
        newColor.a = Mathf.Lerp(minAlpha, originalColor.a, alpha);
        mat.color = newColor;
    }
}
