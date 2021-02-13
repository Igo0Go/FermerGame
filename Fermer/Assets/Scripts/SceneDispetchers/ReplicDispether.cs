﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(AudioSource))]
public class ReplicDispether : MonoBehaviour
{
    [SerializeField] private GameObject replicPanel;
    [SerializeField] private Text replicText;
    [SerializeField] private Toggle useTextToggle;

    private List<ReplicItem> replicas;
    private AudioSource source;
    private ReplicItem bufer;

    public void OnToggleChanged()
    {
        if(bufer != null)
        {
            replicPanel.SetActive(useTextToggle.isOn);
            if (useTextToggle.isOn)
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
    }
    public void ClearList()
    {
        if(source.isPlaying)
            source.Stop();
        replicPanel.SetActive(false);
        replicas.Clear();
    }
    public void AddInList(List<ReplicItem> items)
    {
        replicas.AddRange(items);
        if(bufer == null)
            CheckReplicas();
    }
    public void CheckReplicas()
    {
        source.Stop();
        replicPanel.SetActive(false);
        if (replicas.Count > 0)
            StartReplica();
        else
            bufer = null;
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
        Invoke("CheckReplicas", source.clip.length + 0.3f);
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