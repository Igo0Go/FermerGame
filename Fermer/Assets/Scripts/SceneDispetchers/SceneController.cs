using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public Transform lootSpawnPoit;
    public List<GameObject> randomBots;
    public List<Transform> randomSpawnPoints;
    public AudioSource musicSource;
    public List<AudioClip> randomMusic;
    public List<ReplicItem> randomReplic;

    public GameObject airBot;
    public List<Transform> pointsInAir;
    public List<ReplicPointScript> replicPoints;

    private ReplicDispether replicDispether;
    private int currentWaveNumber;
    private int currentMusicIndex;
    private List<GameObject> currentWave;
    private bool opportunityToCheck;

    public void OnPlayerEntered()
    {
        currentMusicIndex = -1;
        NextMusic();
        Destroy(GetComponent<BoxCollider>());
        NextWave();
        SavedObjects.toArena = true;
    }

    private void NextWave()
    {
        float replicasTime = 0;
        currentWaveNumber++;
        if (currentWaveNumber > 0)
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
            if(number >= pointsInAir.Count)
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
        if(currentWaveNumber > 0)
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
        List<ReplicItem> currentRandomReplic = new List<ReplicItem>() { randomReplic[UnityEngine.Random.Range(0, randomReplic.Count)] };
        replicDispether.AddInList(currentRandomReplic);
        Invoke("ReturnOpportunityToCheck", currentRandomReplic[0].clip.length + 5);
    }

    private void SetPolygone()
    {
        foreach (var item in waves[currentWaveNumber].movingCubes)
        {
            item.ChangePosition();
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
        Setup();
    }

    private void Setup()
    {
        currentWaveNumber = -1;

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
    }
    private void CheckWave()
    {
        if(currentWave != null && opportunityToCheck)
        {
            for (int i = 0; i < currentWave.Count; i++)
            {
                if (currentWave[i] == null)
                {
                    currentWave.Remove(currentWave[i]);
                    i--;
                }
            }
            if (currentWave.Count == 0)
            {
                if (currentWaveNumber < waves.Count && waves[currentWaveNumber].spawnPrefab != null)
                {
                    Instantiate(waves[currentWaveNumber].spawnPrefab, lootSpawnPoit.position, Quaternion.identity);
                }
                opportunityToCheck = false;
                currentWave = null;
                if (currentWaveNumber < waves.Count-1)
                    NextWave();
                else
                    RandomWave();
            }
        }
    }
    private void ReturnOpportunityToCheck() => opportunityToCheck = true;
    private void NextMusic()
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
