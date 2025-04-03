using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBackgroundMusic : MonoBehaviour
{
    public AudioSource localAudioSource;

    void Start()
    {
        var globalMusicManager = FindObjectOfType<MusicManager>();
        if (globalMusicManager != null && globalMusicManager.GetComponent<AudioSource>().isPlaying)
        {
            globalMusicManager.GetComponent<AudioSource>().Stop();
        }

        if (!localAudioSource.isPlaying)
        {
            localAudioSource.Play();
        }
    }
}
