using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public static class SavedObjects
{
    public static GameObject UIDispetcher;
    public static GameObject player;
    public static bool toArena;
}

public class SceneController : MonoBehaviour
{
    public List<Transform> playerStartPos;
    public List<Wave> waves;
    public Transform lootSpawnPoint;
    public List<GameObject> randomBots;
    public List<Transform> randomSpawnPoints;
    public AudioSource musicSource;
    public List<AudioClip> randomMusic;
    public List<ReplicItem> randomReplic;

    public GameObject airBot;
    public List<Transform> pointsInAir;
    public List<ReplicPointScript> replicPoints;

    [SerializeField] private List<GameObject> alarmBots;
    [SerializeField] private AudioClip alarmMusic;
    [SerializeField] private ReplicItem alarmFinalReplica;
    [SerializeField] private List<TranslateScript> movingCubes;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform pointer;
    [SerializeField] private ArenaController arenaController;
    [SerializeField] private GameObject transitCamera;

    private ReplicDispether replicDispether;
    private int currentWaveNumber;
    private int currentMusicIndex;
    private List<GameObject> currentWave;
    private bool opportunityToCheck;
    private bool rayToLast;
    private bool controlMovingCubes;
    private bool alarm;

    private string musicFolderPath = Path.Combine(Application.streamingAssetsPath, "Music");

    public void NoMovingCubesControl() => controlMovingCubes = false;

    public void OnPlayerEntered()
    {
        currentMusicIndex = -1;
        NextMusic();
        Destroy(GetComponent<BoxCollider>());
        NextWave();
        SavedObjects.toArena = true;
    }

    public void Alarm()
    {
        alarm = true;
        if(controlMovingCubes)
        {
            arenaController.AllToDefault();
        }
        currentWave = new List<GameObject>();
        foreach (var item in alarmBots)
        {
            currentWave.Add(Instantiate(item, randomSpawnPoints[UnityEngine.Random.Range(0, randomSpawnPoints.Count)].position, Quaternion.identity));
        }
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = alarmMusic;
        musicSource.Play();
    }

    private void NextWave()
    {
        float replicasTime = 0;
        currentWaveNumber++;
        if (currentWaveNumber > 0 && controlMovingCubes)
        {
            foreach (var item in waves[currentWaveNumber - 1].movingCubes)
            {
                item.ToDefaultPos();
            }
        }
        currentWave = new List<GameObject>();
        Messenger<int>.Broadcast(GameEvent.NEXT_WAVE, currentWaveNumber);
        foreach (var item in waves[currentWaveNumber].bots)
        {
            currentWave.Add(Instantiate(item, randomSpawnPoints[UnityEngine.Random.Range(0, randomSpawnPoints.Count)].position, Quaternion.identity));
        }
        for (int i = 0; i < waves[currentWaveNumber].airBotsCount; i++)
        {
            int number = i;
            if (number >= pointsInAir.Count)
            {
                number -= i;
            }
            currentWave.Add(Instantiate(airBot, pointsInAir[number].position, Quaternion.identity));
        }
        replicDispether.AddInList(waves[currentWaveNumber].voicesForWave);
        for (int i = 0; i < waves[currentWaveNumber].voicesForWave.Count; i++)
        {
            replicasTime += waves[currentWaveNumber].voicesForWave[i].clip.length;
        }
        if (currentWaveNumber > 0 && controlMovingCubes)
        {
            foreach (var item in waves[currentWaveNumber - 1].movingCubes)
            {
                item.ToDefaultPos();
            }
        }
        Invoke("SetPolygone", 5);
        Invoke("ReturnOpportunityToCheck", replicasTime + 5);
    }

    private void RandomWave()
    {
        currentWaveNumber++;
        currentWave = new List<GameObject>();
        Messenger<int>.Broadcast(GameEvent.NEXT_WAVE, currentWaveNumber);
        foreach (var item in randomBots)
        {
            currentWave.Add(Instantiate(item, randomSpawnPoints[UnityEngine.Random.Range(0, randomSpawnPoints.Count)].position, Quaternion.identity));
        }
        foreach (var item in pointsInAir)
        {
            currentWave.Add(Instantiate(airBot, item.position, Quaternion.identity));
        }

        if (controlMovingCubes)
        {
            foreach (var item in movingCubes)
            {
                item.ToDefaultPos();
            }
            foreach (var item in movingCubes)
            {
                if (UnityEngine.Random.Range(0, 2) > 0)
                {
                    item.ChangePosition();
                }
            }
        }

        List<ReplicItem> currentRandomReplic = new List<ReplicItem>() { randomReplic[UnityEngine.Random.Range(0, randomReplic.Count)] };
        replicDispether.AddInList(currentRandomReplic);
        Invoke("ReturnOpportunityToCheck", currentRandomReplic[0].clip.length + 5);
    }

    private void SetPolygone()
    {
        if(controlMovingCubes)
        {
            foreach (var item in waves[currentWaveNumber].movingCubes)
            {
                item.ChangePosition();
            }
        }
    }

    private void Awake()
    {
        Messenger.AddListener(GameEvent.EXIT_LEVEL, Setup);
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.EXIT_LEVEL, Setup);
    }

    private void Start()
    {
        if(TryLoadPlayerMusic(out List<AudioClip> clips))
        {
            randomMusic = clips;
        }
        Setup();
    }

    private void Setup()
    {
        controlMovingCubes = true;
        currentWaveNumber = -1;

        lineRenderer.positionCount = 2;

        if (SavedObjects.UIDispetcher == null)
        {
            SavedObjects.UIDispetcher = Instantiate(playerUI);
        }
        if (SavedObjects.player == null)
        {
            SavedObjects.player = Instantiate(player, Vector3.zero, Quaternion.identity);
        }
        SavedObjects.UIDispetcher.GetComponent<UIDispetcher>().Setup();
        SavedObjects.player.GetComponent<InputMove>().Setup(playerStartPos[SavedObjects.toArena? 1: 0]);
        SavedObjects.player.GetComponent<PlayerInventory>().Setup();

        replicDispether = SavedObjects.UIDispetcher.GetComponent<UIDispetcher>().replicDispether;
        foreach (var item in replicPoints)
        {
            item.replicDispether = replicDispether;
        }
        replicDispether.Setup();
        replicDispether.ClearList();
    }
    private void Update()
    {
        CheckWave();
        RayToLast();
    }
    private void CheckWave()
    {
        if (currentWave != null && opportunityToCheck)
        {
            for (int i = 0; i < currentWave.Count; i++)
            {
                if (currentWave[i] == null)
                {
                    currentWave.Remove(currentWave[i]);
                    i--;
                }
            }

            rayToLast = currentWave.Count == 1;

            if (currentWave.Count == 0)
            {
                if(!alarm)
                {
                    rayToLast = true;
                    if (currentWaveNumber < waves.Count && waves[currentWaveNumber].spawnPrefab != null)
                    {
                        Instantiate(waves[currentWaveNumber].spawnPrefab, lootSpawnPoint.position, Quaternion.identity);
                    }
                    opportunityToCheck = false;
                    currentWave = null;
                    if (currentWaveNumber < waves.Count - 1)
                        NextWave();
                    else
                        RandomWave();
                }
                else
                {
                    opportunityToCheck = false;
                    FinalAlarm();
                }
            }
        }
    }
    private void ReturnOpportunityToCheck() => opportunityToCheck = true;
    private void NextMusic()
    {
        if(!alarm)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
            currentMusicIndex++;
            if (currentMusicIndex >= randomMusic.Count) currentMusicIndex = 0;
            musicSource.clip = randomMusic[currentMusicIndex];
            musicSource.Play();
            Invoke("NextMusic", musicSource.clip.length + 1);
        }
    }

    private void FinalAlarm()
    {
        replicDispether.AddInList(new List<ReplicItem>() { alarmFinalReplica});
        Messenger.Broadcast(GameEvent.START_FINAL_LOADING);
        Invoke("LoadFinalScene", 4);
    }
    private void LoadFinalScene()
    {
        SceneManager.LoadScene(1);
        transitCamera.SetActive(true);
    }

    private void RayToLast()
    {
        if(currentWave != null)
        {
            rayToLast = currentWave.Count == 1;
        }

        if(rayToLast)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, pointer.position);
            lineRenderer.SetPosition(1, currentWave[0].transform.position);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
    private bool TryLoadPlayerMusic(out List<AudioClip> audioClips)
    {
        try
        {
            audioClips = null;
            string[] files = Directory.GetFiles(musicFolderPath);
            if (files.Length == 0)
            {
                return false;
            }
            audioClips = new List<AudioClip>();

            DirectoryInfo di = new DirectoryInfo(musicFolderPath);
            FileInfo[] UserFiles = di.GetFiles("*.ogg", SearchOption.TopDirectoryOnly);
            if (UserFiles.Length > 0)// если массив не пуст
            {
                for (int i = 0; i < UserFiles.Length; i++)
                {
                    WWW www = new WWW(Path.Combine(musicFolderPath, UserFiles[i].Name));
                    AudioClip clip = www.GetAudioClip(false, true, AudioType.OGGVORBIS);
                    audioClips.Add(clip);
                }
            }
        }
        catch
        {
            audioClips = null;
            return false;
        }
        return true;
    }

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerUI;
}

[Serializable]
public class Wave
{
    public List<GameObject> bots;
    public List<TranslateScript> movingCubes;
    public int airBotsCount;
    public GameObject spawnPrefab;
    public List<ReplicItem> voicesForWave;
}
