using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    public List<AudioClip> loadedAudioClips;
    private int clipIndex;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        // shuffle the list
        var count = loadedAudioClips.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = Random.Range(i, count);
            var tmp = loadedAudioClips[i];
            loadedAudioClips[i] = loadedAudioClips[r];
            loadedAudioClips[r] = tmp;
        }
    }

    private void Update()
    {
        audioSource.volume = GameManager.I.IsDriving ? 1f : 0.2f;
        if (GameManager.I.IsDriving)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick2Button1))
            {
                PlayNextSong();
            }
        }
    }

    private void Start()
    {
        PlayNextSong();
    }

    public void PlayNextSong()
    {
        audioSource.Stop();
        audioSource.clip = loadedAudioClips[clipIndex];
        audioSource.Play();
        clipIndex = (clipIndex + 1) % loadedAudioClips.Count;
    }
}
