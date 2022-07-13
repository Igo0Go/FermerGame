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
        StartCoroutine(ToStart());
    }

    public void PlayReplicas()
    {
        GetComponent<BoxCollider>().enabled = false;
        replicDispether.AddInList(replicas);
        replicDispether.replicasEnd.AddListener(OnReplicasEnd);
    }

    private void OnReplicasEnd()
    {
        UseCubes();
        Destroy(gameObject, 2);
    }

    private void UseCubes()
    {
        foreach (var item in movingCubes)
        {
            item.ChangePosition();
        }
    }
    private IEnumerator ToStart()
    {
        yield return new WaitForSeconds(1);
        GetComponent<BoxCollider>().enabled = true;
    }
}
