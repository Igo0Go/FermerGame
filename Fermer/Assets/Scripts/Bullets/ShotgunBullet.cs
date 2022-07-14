using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ShotgunBullet : Bullet //снаряд, который не удаляетс определённое количество столкновений с врагом
{
    [SerializeField, Range(1, 5)] private int numberOfBreaks;

    private SphereCollider coll;

    private void Start()
    {
        coll = GetComponent<SphereCollider>();
    }

    public override void Hit(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Enemy"))
        {
            numberOfBreaks--;
            coll.radius /= 2;
            hit.collider.GetComponent<AliveController>().GetDamage(damage);
            GameController.HIT.Invoke();
            if (numberOfBreaks <= 0)
                Destroy(gameObject);
        }
        else if (hit.collider.CompareTag("Bullet"))
        {
            if (hit.collider.TryGetComponent<TargetTrackerBullet>(out TargetTrackerBullet bullet))
            {
                bullet.Explosion();
                GameController.HIT.Invoke();
            }
        }
        else if (hit.collider.CompareTag("InteractiveBox"))
        {
            hit.collider.GetComponent<InteractiveBox>().OnFightAction();
            GameController.HIT.Invoke();
        }
        else
        {
            GameObject obj = Instantiate(decal);
            obj.transform.position = hit.point;
            obj.transform.forward = hit.normal;
            obj.GetComponent<Decal>().Init(1);
            Destroy(gameObject);
        }
    }
}
