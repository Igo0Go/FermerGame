using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VIP : AliveController
{
    [SerializeField] private List<ReplicItem> onDamageReplicas;
    [SerializeField] private List<ReplicItem> finalReplicas;
    [SerializeField] private UnityEvent onDeath;
    [SerializeField] private UnityEvent onFirstDamage;

    private ReplicDispether replicDispether;

    private void Start()
    {
        Health = maxHealth;
        StartCoroutine(GetReplicDispethcer());
    }

    public override void GetDamage(int damage)
    {
        if(Health == maxHealth)
        {
            onFirstDamage?.Invoke();
        }
        base.GetDamage(damage);
        if (Health > 0)
        {
            PlayRandomDamageReplic();
        }
    }

    public override void Death()
    {
        gameObject.tag = "Untagged";
        replicDispether.ClearList();
        replicDispether.AddInList(finalReplicas);
        onDeath?.Invoke();
        Destroy(this);
    }

    private void PlayRandomDamageReplic()
    {
        if(onDamageReplicas.Count > 0)
        {
            replicDispether.ClearList();
            int index = Random.Range(0, onDamageReplicas.Count);
            replicDispether.AddInList(new List<ReplicItem>() { onDamageReplicas[index] });
            onDamageReplicas.RemoveAt(index);
        }
    }

    private IEnumerator GetReplicDispethcer()
    {
        while(replicDispether == null)
        {
            try
            {
                replicDispether = SavedObjects.UIDispetcher.GetComponent<UIDispetcher>().replicDispether;
            }
            catch
            {

            }
            yield return null;
        }
    }
}