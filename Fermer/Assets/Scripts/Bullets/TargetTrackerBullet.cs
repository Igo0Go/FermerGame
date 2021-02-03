using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTrackerBullet : Bullet //Снаряд, который доворачивается к цели при полёте
{
    [SerializeField, Range(0.01f,1), Tooltip("Скорость отклонения к цели")] private float targetTrakckingSpeed = 0.5f;

    private Transform target;

    public override void MoveBullet()
    {
        if(target != null)
        {
            Vector3 dir = (target.position + Vector3.up) - transform.position;
            if(Vector3.Angle(transform.forward, dir.normalized) > 1)
            {
                transform.forward = Vector3.Lerp(transform.forward, dir.normalized, targetTrakckingSpeed);
            }
        }
        base.MoveBullet();
    }

    public override void Hit(RaycastHit hit)
    {

        if (hit.collider.tag.Equals("Enemy"))
        {
            AliveController character = hit.collider.GetComponent<AliveController>();
            character.GetDamage(damage);
            Messenger.Broadcast(GameEvent.HIT);
        }
        else if(hit.collider.tag.Equals("Player"))
        {
            hit.collider.GetComponent<PlayerCharacter>().OnTakeDamageFromDirection(new Vector2(transform.forward.x, transform.forward.z));
        }

        GameObject obj = Instantiate(decal);
        obj.transform.position = hit.point + hit.normal * 0.3f;
        obj.transform.forward = hit.normal;
        obj.GetComponent<Decal>().Init(1.5f);
        Destroy(gameObject);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
