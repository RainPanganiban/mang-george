using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource SFXsource;

    [Header("Music")]
    public AudioClip backgroundMusic;
    public float musicVolume = 0.2f;

    [Header("Player Sound FX")]
    public AudioClip death;
    public AudioClip firingSound;
    public AudioClip dashSound;
    public AudioClip jump;
    public AudioClip dash;
    public AudioClip hurt1;
    public AudioClip hurt2;
    public AudioClip hurt3;
    public float sfxVolume = 1.0f;

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

    private void Start()
    {
        musicSource.volume = musicVolume;
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXsource.PlayOneShot(clip);
    }

}

