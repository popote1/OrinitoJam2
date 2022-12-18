using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixerGroup AMMusic;
    public AudioMixerGroup AMSFX;
    public AudioSource AsMusic;
    public bool IsPlayingMusic;
    

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
    }


    public void PlaySFX(AudioClip clips, float volume =1) {
        AudioSource audioSource = transform.AddComponent<AudioSource>();
        audioSource.clip = clips;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0;
        audioSource.outputAudioMixerGroup = AMSFX;
        audioSource.Play();
        Destroy(audioSource , clips.length);
    }
    public void PlayMusic(AudioClip clips, float volume =1) {
        
        AsMusic.clip = clips;
        AsMusic.volume = volume;
        AsMusic.spatialBlend = 0;
        AsMusic.outputAudioMixerGroup = AMMusic;
        AsMusic.Play();
        IsPlayingMusic = true;

    }

    public void PauseMusic() {
        AsMusic.Pause();
        IsPlayingMusic = false;
    }

    public void RestartMusic()
    {
        AsMusic.Play();
        IsPlayingMusic = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
