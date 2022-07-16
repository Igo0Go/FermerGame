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

    private void Start()
    {
        source = GetComponent<AudioSource>();

        switch (type)
        {
            case AudioSourceChangerType.Sound:
                GameController.SOUNDS_CHANGED.AddListener(ChangeVolume);
                source.volume = GameController.SoundsVolume;
                break;
            case AudioSourceChangerType.Music:
                GameController.MUSIC_CHANGED.AddListener(ChangeVolume);
                source.volume = GameController.MusicVolume;
                break;
            case AudioSourceChangerType.Voice:
                GameController.VOICE_CHANGED.AddListener(ChangeVolume);
                GameController.PAUSE.AddListener(OnPause);
                source.volume = GameController.VoicesVolume;
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
