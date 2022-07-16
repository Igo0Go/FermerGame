using UnityEngine;

public class Decal : MonoBehaviour
{
    [SerializeField, Min(0.1f)]
    private float lifeTime;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
