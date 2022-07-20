using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

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
        if(Input.GetKeyDown(KeyCode.E) && !pause && usePlayerMusic)
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
        if(GameController.playerMusic == null || GameController.playerMusic.Count == 0)
        {
            string[] files = Directory.GetFiles(musicFolderPath);
            if (files.Length > 0)
            {
                GameController.playerMusic = new List<AudioClip>();

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
                            string clipName = UserFiles[i].Name.Split('.')[0];
                            clip.name = clipName;
                            GameController.playerMusic.Add(clip);
                            GameController.PLAYER_MUSIC_LOAD_CLIP_COMPLETED.Invoke(clipName);
                        }
                        else
                        {
                            GameController.playerMusic = null;
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(2);

        if (GameController.playerMusic != null && GameController.playerMusic.Count > 0)
        {
            Debug.Log("Вся музыка загружена");
            musicList = GameController.playerMusic;
            usePlayerMusic = true;
        }

        GameController.PLAYER_MUSIC_LOADED.Invoke();
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
