using Managers;
using UnityEngine;
using UnityEngine.UI;

public class UIVolumeController : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider uiSlider;

    private void Start()
    {
        // Cargar valores guardados y actualizar sliders
        masterSlider.value = AudioManager.Instance.GetMasterVolume();
        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();
        uiSlider.value = AudioManager.Instance.GetUIVolume();

        // Asignar los eventos de los sliders
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        uiSlider.onValueChanged.AddListener(SetUIVolume);
    }

    // Métodos llamados por los sliders
    public void SetMasterVolume(float volume)
    {
        AudioManager.Instance.SetMasterVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        AudioManager.Instance.SetMusicVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);
    }

    public void SetUIVolume(float volume)
    {
        AudioManager.Instance.SetUIVolume(volume);
    }

    // Opcional: Guardar configuraciones al cerrar el panel
    private void OnDisable()
    {
        AudioManager.Instance.SaveAudioSettings();
    }
}