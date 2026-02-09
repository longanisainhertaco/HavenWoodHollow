using System;
using System.Collections.Generic;
using UnityEngine;

namespace HavenwoodHollow.Audio
{
    /// <summary>
    /// Defines an audio layer type for mixing control.
    /// </summary>
    public enum AudioLayer
    {
        Music,
        SFX,
        Ambience,
        UI
    }

    /// <summary>
    /// Singleton audio manager handling music, sound effects, and ambient audio.
    /// Supports crossfading between music tracks and spatial SFX.
    /// Integrates with TimeManager for day/night music transitions.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        #region Serialized Fields

        [Header("Audio Sources")]
        [Tooltip("Primary music source")]
        [SerializeField] private AudioSource musicSourceA;
        [Tooltip("Secondary music source for crossfading")]
        [SerializeField] private AudioSource musicSourceB;
        [Tooltip("Ambient audio source (loops)")]
        [SerializeField] private AudioSource ambienceSource;
        [Tooltip("UI sound effects source")]
        [SerializeField] private AudioSource uiSource;

        [Header("Music Library")]
        [Tooltip("Day music tracks")]
        [SerializeField] private AudioClip[] dayMusic;
        [Tooltip("Night music tracks")]
        [SerializeField] private AudioClip[] nightMusic;
        [Tooltip("Raid battle music")]
        [SerializeField] private AudioClip raidMusic;
        [Tooltip("Boss battle music")]
        [SerializeField] private AudioClip bossMusic;

        [Header("Ambience Library")]
        [SerializeField] private AudioClip farmDayAmbience;
        [SerializeField] private AudioClip farmNightAmbience;
        [SerializeField] private AudioClip townAmbience;
        [SerializeField] private AudioClip forestAmbience;
        [SerializeField] private AudioClip mineAmbience;

        [Header("Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.7f;
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1.0f;
        [Range(0f, 1f)]
        [SerializeField] private float ambienceVolume = 0.5f;
        [Range(0f, 1f)]
        [SerializeField] private float uiVolume = 0.8f;

        [Tooltip("Duration of music crossfade in seconds")]
        [SerializeField] private float crossfadeDuration = 2.0f;

        #endregion

        #region Private Fields

        private bool isMusicSourceAActive = true;
        private float crossfadeTimer;
        private bool isCrossfading;
        private AudioClip pendingMusicClip;

        /// <summary>Pool of reusable AudioSources for one-shot SFX.</summary>
        private List<AudioSource> sfxPool;

        [SerializeField] private int sfxPoolSize = 16;

        #endregion

        #region Properties

        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = Mathf.Clamp01(value);
                UpdateMusicVolume();
            }
        }

        public float SFXVolume
        {
            get => sfxVolume;
            set => sfxVolume = Mathf.Clamp01(value);
        }

        public float AmbienceVolume
        {
            get => ambienceVolume;
            set
            {
                ambienceVolume = Mathf.Clamp01(value);
                if (ambienceSource != null)
                    ambienceSource.volume = ambienceVolume;
            }
        }

        public float UIVolume
        {
            get => uiVolume;
            set => uiVolume = Mathf.Clamp01(value);
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSFXPool();
        }

        private void Update()
        {
            if (isCrossfading)
            {
                UpdateCrossfade();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Plays a music track with crossfade from the current track.
        /// </summary>
        public void PlayMusic(AudioClip clip)
        {
            if (clip == null) return;

            AudioSource active = isMusicSourceAActive ? musicSourceA : musicSourceB;

            if (active != null && active.clip == clip && active.isPlaying)
                return;

            pendingMusicClip = clip;
            isCrossfading = true;
            crossfadeTimer = 0f;

            AudioSource incoming = isMusicSourceAActive ? musicSourceB : musicSourceA;

            if (incoming != null)
            {
                incoming.clip = clip;
                incoming.volume = 0f;
                incoming.loop = true;
                incoming.Play();
            }
        }

        /// <summary>
        /// Stops all music with a fade out.
        /// </summary>
        public void StopMusic()
        {
            pendingMusicClip = null;
            isCrossfading = true;
            crossfadeTimer = 0f;
        }

        /// <summary>
        /// Plays a one-shot sound effect at the given position.
        /// </summary>
        public void PlaySFX(AudioClip clip, Vector3 position, float volumeScale = 1f)
        {
            if (clip == null) return;

            AudioSource source = GetAvailableSFXSource();
            if (source == null) return;

            source.transform.position = position;
            source.spatialBlend = 1f;
            source.PlayOneShot(clip, sfxVolume * volumeScale);
        }

        /// <summary>
        /// Plays a non-spatial (2D) sound effect.
        /// </summary>
        public void PlaySFX2D(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null) return;

            AudioSource source = GetAvailableSFXSource();
            if (source == null) return;

            source.spatialBlend = 0f;
            source.PlayOneShot(clip, sfxVolume * volumeScale);
        }

        /// <summary>
        /// Plays a UI sound effect.
        /// </summary>
        public void PlayUISound(AudioClip clip)
        {
            if (clip == null || uiSource == null) return;

            uiSource.PlayOneShot(clip, uiVolume);
        }

        /// <summary>
        /// Sets the ambient audio clip and plays it on loop.
        /// </summary>
        public void SetAmbience(AudioClip clip)
        {
            if (ambienceSource == null) return;

            if (ambienceSource.clip == clip && ambienceSource.isPlaying)
                return;

            ambienceSource.clip = clip;
            ambienceSource.volume = ambienceVolume;
            ambienceSource.loop = true;

            if (clip != null)
                ambienceSource.Play();
            else
                ambienceSource.Stop();
        }

        /// <summary>
        /// Transitions to day music and ambience.
        /// </summary>
        public void TransitionToDay()
        {
            if (dayMusic != null && dayMusic.Length > 0)
            {
                int index = UnityEngine.Random.Range(0, dayMusic.Length);
                PlayMusic(dayMusic[index]);
            }

            SetAmbience(farmDayAmbience);
        }

        /// <summary>
        /// Transitions to night music and ambience.
        /// </summary>
        public void TransitionToNight()
        {
            if (nightMusic != null && nightMusic.Length > 0)
            {
                int index = UnityEngine.Random.Range(0, nightMusic.Length);
                PlayMusic(nightMusic[index]);
            }

            SetAmbience(farmNightAmbience);
        }

        /// <summary>
        /// Transitions to raid battle music.
        /// </summary>
        public void TransitionToRaid()
        {
            PlayMusic(raidMusic);
        }

        /// <summary>
        /// Transitions to boss battle music.
        /// </summary>
        public void TransitionToBoss()
        {
            PlayMusic(bossMusic);
        }

        #endregion

        #region Private Methods

        private void InitializeSFXPool()
        {
            sfxPool = new List<AudioSource>();

            for (int i = 0; i < sfxPoolSize; i++)
            {
                GameObject sfxObj = new GameObject($"SFX_Pool_{i}");
                sfxObj.transform.SetParent(transform);
                AudioSource source = sfxObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                sfxPool.Add(source);
            }
        }

        private AudioSource GetAvailableSFXSource()
        {
            for (int i = 0; i < sfxPool.Count; i++)
            {
                if (sfxPool[i] != null && !sfxPool[i].isPlaying)
                    return sfxPool[i];
            }

            Debug.LogWarning("[AudioManager] SFX pool exhausted.");
            return null;
        }

        private void UpdateCrossfade()
        {
            crossfadeTimer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(crossfadeTimer / crossfadeDuration);

            AudioSource outgoing = isMusicSourceAActive ? musicSourceA : musicSourceB;
            AudioSource incoming = isMusicSourceAActive ? musicSourceB : musicSourceA;

            if (outgoing != null)
                outgoing.volume = Mathf.Lerp(musicVolume, 0f, t);

            if (incoming != null && pendingMusicClip != null)
                incoming.volume = Mathf.Lerp(0f, musicVolume, t);

            if (t >= 1f)
            {
                isCrossfading = false;

                if (outgoing != null)
                    outgoing.Stop();

                isMusicSourceAActive = !isMusicSourceAActive;
            }
        }

        private void UpdateMusicVolume()
        {
            AudioSource active = isMusicSourceAActive ? musicSourceA : musicSourceB;

            if (active != null && !isCrossfading)
                active.volume = musicVolume;
        }

        #endregion
    }
}
