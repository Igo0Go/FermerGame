﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioSourceChangerType
{
    Sound,
    Music,
    Voice
}

[RequireComponent(typeof(AudioSource))]
public class AudioSourceVolumeChanger : MonoBehaviour
{
    public AudioSourceChangerType type;

    private AudioSource source;
    private bool usePause;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        switch (type)
        {
            case AudioSourceChangerType.Music:
                GameController.MUSIC_CHANGED.AddListener(ChangeVolume);
                break;
            case AudioSourceChangerType.Sound:
                GameController.SOUNDS_CHANGED.AddListener(ChangeVolume);
                break;
            case AudioSourceChangerType.Voice:
                GameController.VOICE_CHANGED.AddListener(ChangeVolume);
                GameController.PAUSE.AddListener(OnPause);
                break;
        }
    }

    private void Start()
    {
        switch (type)
        {
            case AudioSourceChangerType.Sound:
                source.volume = PlayerPrefs.GetFloat("Sounds", 0.25f);
                break;
            case AudioSourceChangerType.Music:
                source.volume = PlayerPrefs.GetFloat("Music", 0.3f);
                break;
            case AudioSourceChangerType.Voice:
                source.volume = PlayerPrefs.GetFloat("Voices", 1);
                break;
            default:
                break;
        }
    }

    private void ChangeVolume(float value)
    {
        source.volume = value;
    }
    private void OnPause(bool pause)
    {
        if(pause)
        {
            if(source.isPlaying)
            {
                source.Pause();
                usePause = true;
            }
        }
        else
        {
            if(usePause && !source.isPlaying)
            {
                source.Play();
                usePause = false;
            }

        }
    }
}
