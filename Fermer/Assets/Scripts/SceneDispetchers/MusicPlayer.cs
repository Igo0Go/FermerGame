using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public static class PlayerMusicDataHolder
{
    public static List<AudioClip> playerMusic;
}

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private List<AudioClip> musicList;

    private bool pause;
    private int currentMusicIndex;
    private bool usePlayerMusic;
    private readonly string musicFolderPath = Path.Combine(Application.streamingAssetsPath, "Music");

    void Start()
    {
        musicSource.loop = false;
        currentMusicIndex = -1;
        GameController.PAUSE.AddListener(OnPauseToggle);
        StartCoroutine(TryLoadPlayerMusic());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && !pause)
        {
            NextMusic();
        }
    }

    public void StopAllAndStartThisClipAsLoop(AudioClip clip)
    {
        usePlayerMusic = false;
        musicList = null;
        StopAllCoroutines();
        StartPlayMusic(clip);
        musicSource.loop = true;
        GameController.PLAYER_MUSIC_CHANGED.Invoke(string.Empty);
    }

    public void StartPlayList()
    {
        currentMusicIndex = 0;
        StartPlayMusic(musicList[currentMusicIndex]);
        StartCoroutine(Tick());
    }

    private IEnumerator TryLoadPlayerMusic()
    {
        if(PlayerMusicDataHolder.playerMusic == null || PlayerMusicDataHolder.playerMusic.Count == 0)
        {
            string[] files = Directory.GetFiles(musicFolderPath);
            if (files.Length > 0)
            {

                PlayerMusicDataHolder.playerMusic = new List<AudioClip>();

                DirectoryInfo di = new DirectoryInfo(musicFolderPath);
                FileInfo[] UserFiles = di.GetFiles("*.ogg", SearchOption.TopDirectoryOnly);

                if (UserFiles.Length > 0)
                {
                    for (int i = 0; i < UserFiles.Length; i++)
                    {
                        string fileName = Path.Combine(musicFolderPath, UserFiles[i].Name);


                        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fileName,
                            AudioType.OGGVORBIS);
                        var synRes = www.SendWebRequest();

                        yield return synRes;

                        if (www.result == UnityWebRequest.Result.Success && synRes.isDone)
                        {
                            var data = DownloadHandlerAudioClip.GetContent(www);
                            AudioClip clip = data;
                            clip.name = UserFiles[i].Name.Split('.')[0];
                            PlayerMusicDataHolder.playerMusic.Add(clip);
                        }
                        else
                        {
                            PlayerMusicDataHolder.playerMusic = null;
                            break;
                        }
                    }
                }
            }
        }

        if (PlayerMusicDataHolder.playerMusic != null && PlayerMusicDataHolder.playerMusic.Count > 0)
        {
            Debug.Log("Вся музыка загружена");
            musicList = PlayerMusicDataHolder.playerMusic;
            usePlayerMusic = true;
        }
    }

    private void StartPlayMusic(AudioClip clip)
    {
        if (musicSource.isPlaying)
            musicSource.Stop();

        musicSource.clip = clip;
        musicSource.Play();

        if (usePlayerMusic)
            GameController.PLAYER_MUSIC_CHANGED.Invoke(clip.name);
    }

    private void NextMusic()
    {
        if (musicList == null)
            return;

        currentMusicIndex++;
        if (currentMusicIndex >= musicList.Count)
        {
            currentMusicIndex = 0;
        }
        StartPlayMusic(musicList[currentMusicIndex]);
    }

    private void OnPauseToggle(bool value)
    {
        pause = value;
    }

    private IEnumerator Tick()
    {
        while(musicList != null && musicList.Count > 0)
        {
            if(!pause)
            {
                if(!musicSource.isPlaying)
                {
                    NextMusic();
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
