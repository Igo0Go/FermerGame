using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public List<Transform> playerStartPos;
    public List<Wave> waves;
    public Transform lootSpawnPoint;
    public List<GameObject> randomBots;
    public List<Transform> randomSpawnPoints;
    public List<ReplicItem> randomReplic;

    public GameObject airBot;
    public List<Transform> pointsInAir;
    public List<ReplicPointScript> replicPoints;

    [SerializeField] private GameObject teleportPrefab;
    [SerializeField] private MusicPlayer musicPlayer;
    [SerializeField] private List<GameObject> alarmBots;
    [SerializeField] private AudioClip alarmMusic;
    [SerializeField] private ReplicItem alarmFinalReplica;
    [SerializeField] private List<TranslateScript> movingCubes;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform pointer;
    [SerializeField] private ArenaController arenaController;
    [SerializeField] private GameObject transitCamera;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerUI;

    private ReplicDispether replicDispether;
    private int currentWaveNumber;
    [SerializeField]
    private List<GameObject> currentWave;
    private bool rayToLast;
    private bool controlMovingCubes;
    private bool alarm;

    private bool spawnCompleted;
    private bool needKillEnemies;
    private bool needListenDialogue;

    private void Awake()
    {
        GameController.Init();
        GameController.KILL_ENEMY_FROM_WAVE.AddListener(OnEnemyDead);

        needKillEnemies = needListenDialogue = true;
        rayToLast = false;
    }

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        ConsoleEventCenter.Teleport.Execute.AddListener(OnTpToArena);
        ConsoleEventCenter.KillWave.Execute.AddListener(OnKillEnemies);
        lineRenderer.enabled = false;
        controlMovingCubes = true;
        currentWaveNumber = -1;

        lineRenderer.positionCount = 2;

        UIDispetcher ui = playerUI.GetComponent<UIDispetcher>();

        ui.Setup();

        player.GetComponent<InputMove>().Setup(playerStartPos[GameController.toArena ? 1 : 0]);
        player.GetComponent<PlayerInventory>().Setup();

        replicDispether = ui.replicDispether;
        foreach (var item in replicPoints)
        {
            item.replicDispether = replicDispether;
        }
        replicDispether.Setup();
        replicDispether.ClearList();
    }

    public void OnPlayerEntered()
    {
        replicDispether.replicasEnd.AddListener(OnDialogueEnd);

        GameController.toArena = true;
        Destroy(GetComponent<BoxCollider>());

        musicPlayer.StartPlayList();

        needKillEnemies = needListenDialogue = false;
        CheckWave();
    }

    private void OnTpToArena(string place)
    {
        if (string.IsNullOrEmpty(place))
            return;

        if(place.Equals("arena"))
        {
            GameController.toArena = true;
            player.GetComponent<InputMove>().Setup(playerStartPos[1]);
        }
        else if(place.Equals("start"))
        {
            GameController.toArena = false;
            player.GetComponent<InputMove>().Setup(playerStartPos[0]);
        }
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

        if (currentWave.Count == 1 && spawnCompleted)
        {
            StartCoroutine(RayToLastCoroutine());
        }
        if (currentWave.Count == 0 && spawnCompleted)
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
        spawnCompleted = false;
        needListenDialogue = needKillEnemies = true;
        rayToLast = false;
        currentWave = new List<GameObject>();
        currentWaveNumber++;
        GameController.NEXT_WAVE.Invoke(currentWaveNumber);
        replicDispether.replicasEnd.AddListener(OnDialogueEnd);
        GameObject obj = null;
        if (!alarm)
        {
            if (currentWaveNumber < waves.Count && waves[currentWaveNumber].spawnPrefab != null)
            {
                if(waves[currentWaveNumber].spawnPrefab.TryGetComponent(out Enemy e))
                {
                    StartCoroutine(SpawnEnemyCoroutine(waves[currentWaveNumber].spawnPrefab, lootSpawnPoint.position));
                }
                else
                {
                    Instantiate(waves[currentWaveNumber].spawnPrefab, lootSpawnPoint.position, Quaternion.identity);
                }
            }

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
        musicPlayer.StopAllAndStartThisClipAsLoop(alarmMusic);

        alarm = true;
        if(controlMovingCubes)
        {
            arenaController.AllToDefault();
        }
        foreach (var item in alarmBots)
        {
            currentWave.Add(Instantiate(item, 
                randomSpawnPoints[UnityEngine.Random.Range(0, randomSpawnPoints.Count)].position, Quaternion.identity));
        }
        spawnCompleted = true;
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
        if(currentWaveNumber == 0)
        {
            StartCoroutine(SpawnEnemyWaveWhileDialogueCoroutine(enemies));
        }
        else
        {
            StartCoroutine(SpawnEnemyWaveCoroutine(enemies));
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

    private IEnumerator SpawnEnemyWaveCoroutine(List<GameObject> enemies)
    {
        foreach (var item in enemies)
        {
            Vector3 pos = randomSpawnPoints[UnityEngine.Random.Range(0, randomSpawnPoints.Count)].position;

            Instantiate(teleportPrefab, pos, Quaternion.identity);

            yield return new WaitForSeconds(1);

            currentWave.Add(Instantiate(item, pos, Quaternion.identity));
        }
        spawnCompleted = true;
    }
    private IEnumerator SpawnEnemyWaveWhileDialogueCoroutine(List<GameObject> enemies)
    {
        int currentEnemyNumber = 0;
        while (needListenDialogue)
        {
            GameObject item = enemies[currentEnemyNumber];
            Vector3 pos = randomSpawnPoints[UnityEngine.Random.Range(0, randomSpawnPoints.Count)].position;
            Instantiate(teleportPrefab, pos, Quaternion.identity);
            yield return new WaitForSeconds(1);
            currentWave.Add(Instantiate(item, pos, Quaternion.identity));

            currentEnemyNumber++;
            if(currentEnemyNumber > enemies.Count -1)
            {
                currentEnemyNumber = 0;
            }
            yield return new WaitForSeconds(2);
        }
        spawnCompleted = true;
    }
    private IEnumerator SpawnEnemyCoroutine(GameObject enemy, Vector3 pos)
    {
        Instantiate(teleportPrefab, pos, Quaternion.identity);

        yield return new WaitForSeconds(1);

        currentWave.Add(Instantiate(enemy, pos, Quaternion.identity));
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
