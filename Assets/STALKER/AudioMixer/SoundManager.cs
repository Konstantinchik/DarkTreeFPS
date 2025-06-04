using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Mixer")]
    public AudioMixer audioMixer;

    private const string MASTER_VOL = "MasterVolume";
    private const string MUSIC_VOL = "MusicVolume";
    private const string SFX_VOL = "SFXVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVolumeSettings();
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat(MASTER_VOL, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MASTER_VOL, value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat(MUSIC_VOL, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MUSIC_VOL, value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat(SFX_VOL, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(SFX_VOL, value);
    }

    private void LoadVolumeSettings()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MASTER_VOL, 0.75f));
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_VOL, 0.75f));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_VOL, 0.75f));
    }
}

