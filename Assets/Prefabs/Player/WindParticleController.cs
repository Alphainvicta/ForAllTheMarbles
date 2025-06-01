using UnityEngine;

public class WindParticleController : MonoBehaviour
{
    public float windStrength = 1f;
    public float noiseSpeed = 1f;
    public float noiseMagnitude = 1f;
    public Transform target; // El objeto que recibe el viento (normalmente está quieto)

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void LateUpdate()
    {
        int aliveCount = ps.GetParticles(particles);
        float t = Time.time * noiseSpeed;

        for (int i = 0; i < aliveCount; i++)
        {
            // Dirección base hacia el objeto (target)
            Vector3 toTarget = (target.position - particles[i].position).normalized;

            // Turbulencia con Perlin Noise
            float noiseX = (Mathf.PerlinNoise(t + i * 0.1f, 0f) - 0.5f) * 2f;
            float noiseY = (Mathf.PerlinNoise(0f, t + i * 0.1f) - 0.5f) * 2f;
            Vector3 turbulence = new Vector3(noiseX, noiseY, 0f) * noiseMagnitude;

            Vector3 finalDirection = (toTarget + turbulence).normalized;

            particles[i].velocity = finalDirection * windStrength;
        }

        ps.SetParticles(particles, aliveCount);
    }
}
