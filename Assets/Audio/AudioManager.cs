using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("Audio Mixer")]
        public AudioMixer masterMixer;
        
        [Header("Mixer Groups")]
        public AudioMixerGroup musicGroup;
        public AudioMixerGroup sfxGroup;
        public AudioMixerGroup uiGroup;
        
        [System.Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            [Range(0.1f, 3f)] public float pitch = 1f;
            public bool loop = false;
            public AudioMixerGroup mixerGroup;
            
            [HideInInspector] public AudioSource source;
        }
        
        public List<Sound> sounds = new List<Sound>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                foreach (Sound s in sounds)
                {
                    s.source = gameObject.AddComponent<AudioSource>();
                    s.source.clip = s.clip;
                    s.source.volume = s.volume;
                    s.source.pitch = s.pitch;
                    s.source.loop = s.loop;
                    s.source.outputAudioMixerGroup = s.mixerGroup;
                }
                
                LoadAudioSettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Play(string soundName)
        {
            Sound s = sounds.Find(sound => sound.name == soundName);
            if (s != null)
            {
                s.source.Play();
            }
            else
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
            }
        }
        // En tu AudioManager.cs
public void Pause(string soundName)
{
    Sound s = sounds.Find(sound => sound.name == soundName);
    if (s != null)
    {
        s.source.Pause();
    }
}

public void Unpause(string soundName)
{
    Sound s = sounds.Find(sound => sound.name == soundName);
    if (s != null)
    {
        s.source.UnPause();
    }
}

public bool IsPlaying(string soundName)
{
    Sound s = sounds.Find(sound => sound.name == soundName);
    return s != null && s.source.isPlaying;
}
        public void Stop(string soundName)
        {
            Sound s = sounds.Find(sound => sound.name == soundName);
            if (s != null)
            {
                s.source.Stop();
            }
        }
        
        // Control de volumen
        public void SetMasterVolume(float volume)
        {
            masterMixer.SetFloat("MasterVolume", LinearToDecibel(volume));
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
        
        public void SetMusicVolume(float volume)
        {
            masterMixer.SetFloat("MusicVolume", LinearToDecibel(volume));
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
        
        public void SetSFXVolume(float volume)
        {
            masterMixer.SetFloat("SFXVolume", LinearToDecibel(volume));
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
        
        public void SetUIVolume(float volume)
        {
            masterMixer.SetFloat("UIVolume", LinearToDecibel(volume));
            PlayerPrefs.SetFloat("UIVolume", volume);
        }
        
        private float LinearToDecibel(float linear)
        {
            return linear != 0 ? 20f * Mathf.Log10(linear) : -80f;
        }
        
        private void LoadAudioSettings()
        {
            SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1f));
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
            SetUIVolume(PlayerPrefs.GetFloat("UIVolume", 1f));
        }
        
        public void SaveAudioSettings()
        {
            PlayerPrefs.Save();
        }
        
        public float GetMasterVolume()
        {
            float volume;
            masterMixer.GetFloat("MasterVolume", out volume);
            return DecibelToLinear(volume);
        }
        
        public float GetMusicVolume()
        {
            float volume;
            masterMixer.GetFloat("MusicVolume", out volume);
            return DecibelToLinear(volume);
        }
        
        public float GetSFXVolume()
        {
            float volume;
            masterMixer.GetFloat("SFXVolume", out volume);
            return DecibelToLinear(volume);
        }
        
        public float GetUIVolume()
        {
            float volume;
            masterMixer.GetFloat("UIVolume", out volume);
            return DecibelToLinear(volume);
        }
        
        private float DecibelToLinear(float dB)
        {
            return Mathf.Pow(10f, dB / 20f);
        }
    }
}