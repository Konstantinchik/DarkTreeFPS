using UnityEngine;
using UnityEngine.Audio;

namespace DarkTreeFPS
{
    public class SoundController : MonoBehaviour
    {
        public static SoundController Instance;

        [Header("Mixer")]
        public AudioMixer audioMixer;

        private const string MASTER_VOL = "MasterVolume";
        private const string MUSIC_VOL = "MusicVolume";
        private const string SFX_VOL = "SFXVolume";

        [Header("Audio Settings")]
        public AudioMixerGroup musicMixerGroup; // назначить в инспекторе
        public AudioClip menuMusic;             // назначить в инспекторе
        public AudioSource audioSource;
        public bool loop = true;                // Включать зацикливание музыки

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

        private void Start()
        {
            //PlayerPrefs.DeleteAll();

            // Инициализация аудио
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = musicMixerGroup;
                audioSource.loop = loop;
            }
        }

        public void PlayMenuMusic()
        {
            
            if (menuMusic == null) return;

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = musicMixerGroup; // если нужно
            }

            if (audioSource.clip == menuMusic && audioSource.isPlaying)
                return;

            audioSource.clip = menuMusic;
            audioSource.loop = loop;
            audioSource.Play();
        }

        public void StopMenuMusic()
        {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
        }

        #region [SET VOLUMES]
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
        #endregion

        private void LoadVolumeSettings()
        {
            SetMasterVolume(PlayerPrefs.GetFloat(MASTER_VOL, 0.75f));
            SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_VOL, 0.75f));
            SetSFXVolume(PlayerPrefs.GetFloat(SFX_VOL, 0.75f));
        }
    }
}