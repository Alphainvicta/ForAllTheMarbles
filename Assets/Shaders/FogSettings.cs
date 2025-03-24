using UnityEngine;

[CreateAssetMenu(fileName = "FogSettings", menuName = "Fog/FogSettings")]
public class FogSettings : ScriptableObject
{
    [Header("Fog Properties")]
    public Color fogColor = Color.gray;
    public float fogDensity = 1.0f;
    public float fogStartDistance = 10.0f;
    public float fogEndDistance = 50.0f;

    
    public void ApplyToShader(Material fogMaterial)
    {
        fogMaterial.SetColor("_FogColor", fogColor);
        fogMaterial.SetFloat("_FogDensity", fogDensity);
        fogMaterial.SetFloat("_FogStartDistance", fogStartDistance);
        fogMaterial.SetFloat("_FogEndDistance", fogEndDistance);
    }
}