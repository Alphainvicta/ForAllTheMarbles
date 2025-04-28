using UnityEngine;
using System.Collections.Generic;

public class ParticleSystemManager : MonoBehaviour
{
    public static ParticleSystemManager Instance { get; private set; }
    
    private List<ParticleSystem> allParticles = new List<ParticleSystem>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RegisterParticleSystem(ParticleSystem ps)
    {
        if (!allParticles.Contains(ps))
        {
            allParticles.Add(ps);
        }
    }
    
    public void UnregisterParticleSystem(ParticleSystem ps)
    {
        if (allParticles.Contains(ps))
        {
            allParticles.Remove(ps);
        }
    }
    
    public void PauseAllParticles()
    {
        foreach (var ps in allParticles)
        {
            if (ps != null && ps.isPlaying)
            {
                ps.Pause();
            }
        }
    }
    
    public void ResumeAllParticles()
    {
        foreach (var ps in allParticles)
        {
            if (ps != null && ps.isPaused)
            {
                ps.Play();
            }
        }
    }
    
    public void StopAllParticles()
    {
        foreach (var ps in allParticles)
        {
            if (ps != null)
            {
                ps.Stop();
            }
        }
    }
}