using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ReplicPointScript : MonoBehaviour
{
    [HideInInspector]public ReplicDispether replicDispether;
    public List<ReplicItem> replicas;
    public List<TranslateScript> movingCubes;

    public void PlayReplicas()
    {
        replicDispether.AddInList(replicas);
        replicDispether.replicasEnd.AddListener(OnReplicasEnd);
        GetComponent<BoxCollider>().enabled = false;
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
}
