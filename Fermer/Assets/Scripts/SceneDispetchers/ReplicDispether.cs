using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class ReplicDispether : MonoBehaviour
{
    [SerializeField] private GameObject replicPanel;
    [SerializeField] private Text replicText;
    [SerializeField] private Toggle useTextToggle;

    [HideInInspector]
    public UnityEvent replicasEnd;

    private List<ReplicItem> replicas;
    private AudioSource source;
    private ReplicItem bufer;

    private void Awake()
    {
        replicasEnd = new UnityEvent();
        useTextToggle.isOn = GameController.useSubTitles;
        bufer = null;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(bufer != null)
                StopAll();
        }
    }

    public void OnToggleChanged()
    {
        if(bufer != null)
        {
            GameController.useSubTitles = useTextToggle.isOn;
            replicPanel.SetActive(GameController.useSubTitles);
            if (GameController.useSubTitles)
            {
                replicText.text = bufer.replicText;
                replicText.color = bufer.textColor;
            }
        }
    }
    public void Setup()
    {
        replicas = new List<ReplicItem>();
        source = GetComponent<AudioSource>();
        replicPanel.SetActive(false);
    }
    public void ClearList()
    {
        replicas.Clear();
    }
    public void AddInList(List<ReplicItem> items)
    {
        replicas.AddRange(items);
        if(bufer == null)
            StartCoroutine(CheckReplicas(0));
    }
    public void StopAll()
    {
        source.Stop();
        StopAllCoroutines();
        bufer = null;
        ClearList();
        replicasEnd?.Invoke();
        replicasEnd = new UnityEvent();
        replicPanel.SetActive(false);
    }
    public IEnumerator CheckReplicas(float time)
    {
        yield return new WaitForSeconds(time);
        source.Stop();
        replicPanel.SetActive(false);
        if (replicas.Count > 0)
            StartReplica();
        else
        {
            replicasEnd?.Invoke();
            replicasEnd = new UnityEvent();
            bufer = null;
        }
    }
    private void StartReplica()
    {
        bufer = replicas[0];
        source.clip = bufer.clip;
        if(useTextToggle.isOn)
        {
            replicPanel.SetActive(true);
            replicText.text = bufer.replicText;
            replicText.color = bufer.textColor;
        }
        source.Play();
        StartCoroutine(CheckReplicas(source.clip.length + 0.3f));
        replicas.Remove(replicas[0]);
    }
}

[Serializable]
public class ReplicItem
{
    public AudioClip clip;
    public Color textColor;
    public string replicText;
}
