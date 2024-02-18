using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// A simple AudioManager class for sound effects. Handles playing one shots, and round robins.
/// </summary>
public class AudioManager : MonoBehaviour
{
    #region Singleton

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (transform.root == transform)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    [SerializeField] AudioClip buttonClickSFX;
    [SerializeField] bool pitchReactsToTimescale;

    [SerializeField] AudioMixerGroup mixerGroup;
    [SerializeField] AudioMixer masterMixer;

    private static AudioSource[] sources;
    private const int maxSources = 5;

    private void OnEnable()
    {
        // Settings.OnSettingsChanged += OnSettingsChanged;
    }
    private void OnDisable()
    {
        // Settings.OnSettingsChanged -= OnSettingsChanged;
    }
    private void OnSettingsChanged()
    {
        // masterMixer.SetFloat("MusicVolume", Mathf.Log10(Settings.MusicVolume) * 20);
        // masterMixer.SetFloat("SFXVolume", Mathf.Log10(Settings.SFXVolume) * 20);
    }

    private void Start()
    {
        sources = new AudioSource[maxSources];
        for (int i = 0; i < maxSources; i++)
        {
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].outputAudioMixerGroup = mixerGroup;
        }
    }

    private void Update()
    {
        /**
        if (pitchReactsToTimescale)
        {
            foreach (AudioSource source in sources)
            {
                source.pitch = TimeManager.Paused ? 1 : Time.timeScale;
            }
        }
        **/
    }

    private static AudioSource GetNextAvailableAudioSource()
    {
        foreach (AudioSource source in sources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return sources[0];
    }

    /// <summary>
    /// Plays a predefined button click sound effect.
    /// </summary>
    public static void PlayButtonClick(float volume = 1f)
    {
        PlayAudioClip(Instance.buttonClickSFX, volume);
    }

    /// <summary>
    /// Plays a specified audioclip at the AudioManager's position.
    /// </summary>
    /// <param name="clip">The clip to play.</param>
    public static void PlayAudioClip(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
            return;

        AudioSource source = GetNextAvailableAudioSource();
        source.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// Plays a random clip of a specified array of AudioClips at the AudioManager's position.
    /// </summary>
    /// <param name="clips">The array to choose a clip from.</param>
    public static void PlayRoundRobin(AudioClip[] clips, float volume = 1f)
    {
        PlayAudioClip(clips[Random.Range(0, clips.Length)], volume);
    }
}