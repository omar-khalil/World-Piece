using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] soundtrack;

    private AudioSource audioSource;
    private int currentMusicIndex = 0;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        if (!audioSource.playOnAwake)
        {
            audioSource.clip = soundtrack[0];
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            currentMusicIndex++;
            currentMusicIndex %= soundtrack.Length;
            audioSource.clip = soundtrack[currentMusicIndex];
            audioSource.Play();
        }
    }
}