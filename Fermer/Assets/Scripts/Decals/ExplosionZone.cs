using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class ExplosionZone : MonoBehaviour
{
    [Range(1,100)] public int damage = 10;
    [Range(1, 100)] public float force = 10;
    [SerializeField, Min(0)] private float scaleChangeSpeed;
    [SerializeField, Min(0)] private float maxRange;

    private Transform MyTransform;

    private void Start()
    {
        MyTransform = transform;
        MyTransform.parent = null;
        StartCoroutine(ChangeScaleCoroutine());
    }

    private IEnumerator ChangeScaleCoroutine()
    {
        float t = 0;

        while (t < maxRange)
        {
            t += Time.deltaTime * scaleChangeSpeed;
            MyTransform.localScale = Vector3.one * t;
            MyTransform.Rotate(Vector3.up, 10 * scaleChangeSpeed);
            yield return null;
        }

        while (t > 0)
        {
            t -= Time.deltaTime * scaleChangeSpeed * 4;
            MyTransform.localScale = Vector3.one * t;
            MyTransform.Rotate(Vector3.up, 10 * scaleChangeSpeed);
            yield return null;
        }

        Destroy(gameObject);
    }

}
