using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    [Header("Make sure it's between 0 and 1")]
    public float masterVolume;
    public static SoundManager instance = null;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    //public AudioClip[] clips;
    public AudioClip invalidClip;
    public AudioClip pickupClip;
    public AudioClip rotateClip;
    public AudioClip matchOk;
    public AudioClip matchGood;
    public AudioClip pop;
    public AudioClip coin;
    public AudioClip powerButton;
    public AudioClip powerEffect;

    //private AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //PlaySound(clips[0], 1f, 0.5f, true);
        //audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip audioClip, bool variedPitch)
    {
        PlaySound(audioClip, variedPitch, 1f);
    }

    public void PlaySound(AudioClip audioClip, bool variedPitch, float volume)
    {
        PlaySound(audioClip, variedPitch ? Random.Range(minPitch, maxPitch) : 1f, volume, false);
    }

    public void PlaySound(AudioClip audioClip, float pitch, float volume, bool loop)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>(); //Dynamically stack sounds. Inefficient? 
        audioSource.clip = audioClip;
        audioSource.volume = volume * masterVolume;
        audioSource.pitch = pitch;
        audioSource.Play();
        audioSource.loop = loop;
        Destroy(audioSource, audioClip.length);
    }
}
