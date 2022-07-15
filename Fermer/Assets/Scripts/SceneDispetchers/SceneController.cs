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
    [SerializeField]
    private List<GameObject> currentWave;
    private bool rayToLast;
    private bool controlMovingCubes;
    private bool alarm;

    private bool needKillEnemies;
    private bool needListenDialogue;

    private readonly string musicFolderPath = Path.Combine(Application.streamingAssetsPath, "Music");

    private void Awake()
    {
        GameController.Init();
        GameController.KILL_ENEMY_FROM_WAVE.AddListener(OnEnemyDead);

        needKillEnemies = needListenDialogue = true;
        rayToLast = false;
    }

    private void Start()
    {
        if (TryLoadPlayerMusic(out List<AudioClip> clips))
        {
            randomMusic = clips;
        }
        Setup();
    }

    private void Setup()
    {
        ConsoleEventCenter.TeleportToArena.Execute.AddListener(OnTpToArena);
        ConsoleEventCenter.KillWave.Execute.AddListener(OnKillEnemies);
        lineRenderer.enabled = false;
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
        SavedObjects.player.GetComponent<InputMove>().Setup(playerStartPos[SavedObjects.toArena ? 1 : 0]);
        SavedObjects.player.GetComponent<PlayerInventory>().Setup();

        replicDispether = SavedObjects.UIDispetcher.GetComponent<UIDispetcher>().replicDispether;
        foreach (var item in replicPoints)
        {
            item.replicDispether = replicDispether;
        }
        replicDispether.Setup();
        replicDispether.ClearList();
    }

    public void OnPlayerEntered()
    {
        NextMusic();

        replicDispether.replicasEnd.AddListener(OnDialogueEnd);

        SavedObjects.toArena = true;
        Destroy(GetComponent<BoxCollider>());

        currentMusicIndex = -1;
        needKillEnemies = needListenDialogue = false;
        CheckWave();
    }

    private void OnTpToArena()
    {
        SavedObjects.toArena = true;
        SavedObjects.player.GetComponent<InputMove>().Setup(playerStartPos[SavedObjects.toArena ? 1 : 0]);
    }
    private void OnKillEnemies()
    {
        StartCoroutine(KillWaveCoroutine());
    }

    private void OnEnemyDead(GameObject enemy)
    {
        if (currentWave == null)
            return;

        if (currentWave.Contains(enemy))
        {
            currentWave.Remove(enemy);
        }

        if (currentWave.Count == 1)
        {
            StartCoroutine(RayToLastCoroutine());
        }
        if (currentWave.Count == 0)
        {
            rayToLast = false;
            needKillEnemies = false;
            if (needListenDialogue)
                StartCoroutine(CheckWaveCoroutine());
        }
    }
    private void OnDialogueEnd()
    {
        needListenDialogue = false;

        if (needKillEnemies)
            StartCoroutine(CheckWaveCoroutine());
    }

    private void CheckWave()
    {
        needListenDialogue = needKillEnemies = true;
        rayToLast = false;
        currentWaveNumber++;
        GameController.NEXT_WAVE.Invoke(currentWaveNumber);
        replicDispether.replicasEnd.AddListener(OnDialogueEnd);
        GameObject obj = null;
        if (!alarm)
        {

            if (currentWaveNumber < waves.Count && waves[currentWaveNumber].spawnPrefab != null)
            {
                obj = Instantiate(waves[currentWaveNumber].spawnPrefab, lootSpawnPoint.position, Quaternion.identity);
            }
            currentWave = null;

            if (currentWaveNumber < waves.Count)
            {
                ScenarioWave();
            }
            else
            {
                RandomWave();
            }
        }
        else
        {
            FinalAlarm();
        }

        if(obj != null && obj.TryGetComponent(out Enemy enemy))
        {
            currentWave.Add(obj);
        }
    }

    private void ScenarioWave()
    {
        ChangeCubes();
        SpawnEnemies(waves[currentWaveNumber].bots);
        SpawnAirBots();
        replicDispether.AddInList(waves[currentWaveNumber].voicesForWave);
    }
    private void RandomWave()
    {
        ChangeCubesRandom();
        SpawnEnemies(randomBots);
        SpawnAirBots();
        List<ReplicItem> currentRandomReplic = new List<ReplicItem>() { randomReplic[UnityEngine.Random.Range(0, randomReplic.Count)] };
        replicDispether.AddInList(currentRandomReplic);
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
            currentWave.Add(Instantiate(item, 
                randomSpawnPoints[UnityEngine.Random.Range(0, randomSpawnPoints.Count)].position, Quaternion.identity));
        }
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = alarmMusic;
        musicSource.Play();
    }
    private void FinalAlarm()
    {
        replicDispether.AddInList(new List<ReplicItem>() { alarmFinalReplica });
        GameController.START_FINAL_LOADING.Invoke();
        Invoke(nameof(LoadFinalScene), 4);
    }

    private void ChangeCubes()
    {
        if (currentWaveNumber > 0 && controlMovingCubes)
        {
            foreach (var item in waves[currentWaveNumber - 1].movingCubes)
            {
                item.ToDefaultPos();
            }
        }
        Invoke(nameof(SetPolygone), 5);
    }
    private void ChangeCubesRandom()
    {
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
    }
    private void SpawnEnemies(List<GameObject> enemies)
    {
        currentWave = new List<GameObject>();
        foreach (var item in enemies)
        {
            currentWave.Add(Instantiate(item,
                randomSpawnPoints[UnityEngine.Random.Range(0, randomSpawnPoints.Count)].position, Quaternion.identity));
        }
    }
    private void SpawnAirBots()
    {
        int airBotsCount = currentWaveNumber < waves.Count ? waves[currentWaveNumber].airBotsCount : 5;

        for (int i = 0; i < airBotsCount; i++)
        {
            int number = i;
            if (number >= pointsInAir.Count)
            {
                number -= i;
            }
            currentWave.Add(Instantiate(airBot, pointsInAir[number].position, Quaternion.identity));
        }
    }

    private void SetPolygone()
    {
        if (controlMovingCubes)
        {
            foreach (var item in waves[currentWaveNumber].movingCubes)
            {
                item.ChangePosition();
            }
        }
    }
    public void NoMovingCubesControl() => controlMovingCubes = false;
    private void LoadFinalScene()
    {
        SceneManager.LoadScene(1);
        transitCamera.SetActive(true);
    }

    private void NextMusic()
    {
        if (!alarm)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
            currentMusicIndex++;
            if (currentMusicIndex >= randomMusic.Count) currentMusicIndex = 0;
            musicSource.clip = randomMusic[currentMusicIndex];
            musicSource.Play();
            Invoke(nameof(NextMusic), musicSource.clip.length + 1);
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

    private IEnumerator KillWaveCoroutine()
    {
        StopCoroutine(CheckWaveCoroutine());

        bool bufer = needListenDialogue;
        needListenDialogue = true;

        if (currentWaveNumber >= 0)
        {
            for (int i = 0; i < currentWave.Count; i++)
            {
                if (currentWave[i] != null)
                {
                    currentWave[i].GetComponent<Enemy>().Death();
                    i--;
                }
            }
        }
        yield return new WaitForSeconds(1);
        needListenDialogue = bufer;
    }
    private IEnumerator CheckWaveCoroutine()
    {
        while (true)
        {
            if (needKillEnemies || needListenDialogue)
                yield return new WaitForSeconds(1);
            else
            {
                CheckWave();
                break;
            }
        }
    }
    private IEnumerator RayToLastCoroutine()
    {
        rayToLast = true;
        lineRenderer.enabled = true;

        while (rayToLast)
        {
            if (currentWave == null || currentWave[0] == null)
                break;

            lineRenderer.SetPosition(0, pointer.position);
            lineRenderer.SetPosition(1, currentWave[0].transform.position);
            yield return null;
        }
        lineRenderer.enabled = false;
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
