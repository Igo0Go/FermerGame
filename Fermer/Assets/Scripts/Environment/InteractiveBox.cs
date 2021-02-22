using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveBox : MonoBehaviour
{
    [SerializeField]
    private GameObject loot;

    [SerializeField]
    private GameObject wreckage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Blade"))
        {
            OnFightAction();
        }
    }

    public virtual void OnFightAction()
    {
        if(loot != null)
        {
            Vector3 dir = new Vector3(Random.Range(-0.05f, 0.05f), 2, Random.Range(-0.05f, 0.05f));
            Instantiate(loot, transform.position + dir, Quaternion.identity).GetComponent<Rigidbody>()
                .AddForce(dir, ForceMode.Impulse);
        }
        Instantiate(wreckage, transform.position, Quaternion.identity).GetComponent<Decal>().Init(2);
        Destroy(gameObject);
    }
}
