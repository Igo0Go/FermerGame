using System.Collections;
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
                Messenger<float>.AddListener(GameEvent.MUSIC_CHANGED, ChangeVolume);
                break;
            case AudioSourceChangerType.Sound:
                Messenger<float>.AddListener(GameEvent.SOUNDS_CHANGED, ChangeVolume);
                break;
            case AudioSourceChangerType.Voice:
                Messenger<float>.AddListener(GameEvent.VOICE_CHANGED, ChangeVolume);
                Messenger<bool>.AddListener(GameEvent.PAUSE, OnPause);
                break;

        }
     //   Messenger.AddListener(GameEvent.EXIT_LEVEL, OnDestroy);
    }
    private void OnDestroy()
    {
        switch (type)
        {
            case AudioSourceChangerType.Music:
                Messenger<float>.RemoveListener(GameEvent.MUSIC_CHANGED, ChangeVolume);
                break;
            case AudioSourceChangerType.Sound:
                Messenger<float>.RemoveListener(GameEvent.SOUNDS_CHANGED, ChangeVolume);
                break;
            case AudioSourceChangerType.Voice:
                Messenger<float>.RemoveListener(GameEvent.VOICE_CHANGED, ChangeVolume);
                Messenger<bool>.RemoveListener(GameEvent.PAUSE, OnPause);
                break;
        }
       // Messenger.RemoveListener(GameEvent.EXIT_LEVEL, OnDestroy);
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
