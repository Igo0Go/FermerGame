using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ReplicPointScript : MonoBehaviour
{
    [HideInInspector]public ReplicDispether replicDispether;
    public List<ReplicItem> replicas;
    public List<TranslateScript> movingCubes;

    private void Awake()
    {
        GetComponent<BoxCollider>().enabled = false;
    }

    private void Start()
    {
        Invoke("ToStart", 1);
    }

    public void PlayReplicas()
    {
        GetComponent<BoxCollider>().enabled = false;
        replicDispether.AddInList(replicas);

        float delayTime = 0;

        foreach (var item in replicas)
        {
            delayTime += item.clip.length;
        }
        delayTime += 2;
        Invoke("UseCubes", delayTime);
        Destroy(gameObject, delayTime + 1);
    }

    private void UseCubes()
    {
        foreach (var item in movingCubes)
        {
            item.ChangePosition();
        }
    }
    private void ToStart()
    {
        GetComponent<BoxCollider>().enabled = true;
    }
}
