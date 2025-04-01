using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXsource;

    [Header("Music")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float musicVolume = 0.2f;

    [Header("Player Sound FX")]
    public AudioClip death;
    public AudioClip firingSound;
    public AudioClip jump;
    public AudioClip dash;
    public AudioClip hurt1;
    public AudioClip hurt2;
    public AudioClip hurt3;
    [Range(0f, 1f)] public float sfxVolume = 1.0f;

    [Header("Enemy Sound FX")]
    public AudioClip attackingSound1;
    public AudioClip attackingSound2;
    public AudioClip attackingSound3;
    public AudioClip attackingSound5;
    public AudioClip attackingSound6;
    public AudioClip transition;
    public AudioClip deathEnemy;
    public AudioClip intro;
    [Range(0f, 1f)] public float enemySFXVolume = 1.0f;

    private void Start()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            SFXsource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlayEnemySFX(AudioClip clip)
    {
        if (clip != null)
        {
            SFXsource.PlayOneShot(clip, enemySFXVolume);
        }
    }
}
