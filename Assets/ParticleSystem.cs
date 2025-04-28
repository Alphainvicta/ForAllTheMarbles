using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ManagedParticleSystem : MonoBehaviour
{
    private ParticleSystem ps;
    
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        ParticleSystemManager.Instance.RegisterParticleSystem(ps);
    }
    
    private void OnDestroy()
    {
        if (ParticleSystemManager.Instance != null)
        {
            ParticleSystemManager.Instance.UnregisterParticleSystem(ps);
        }
    }
}