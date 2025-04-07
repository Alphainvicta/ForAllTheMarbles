using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DistanceFade : MonoBehaviour
{
    public float fadeStart = 50f;
    public float fadeEnd = 10f;

    private Material originalMaterial;
    private Material fadeMaterial;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;
        
        // Crear una copia del material con transparencia
        fadeMaterial = new Material(originalMaterial);
        fadeMaterial.shader = Shader.Find("Standard"); // O el shader que uses
        rend.material = fadeMaterial;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
        float alpha = Mathf.Clamp01(1 - (dist - fadeEnd) / (fadeStart - fadeEnd));
        
        Color color = fadeMaterial.color;
        color.a = alpha;
        fadeMaterial.color = color;
    }

    void OnDestroy()
    {
        if (fadeMaterial != null)
            Destroy(fadeMaterial);
    }
}