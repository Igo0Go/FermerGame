﻿using System.Collections;
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
        if (hit.collider.tag.Equals("Enemy"))
        {
            numberOfBreaks--;
            coll.radius /= 2;
            hit.collider.GetComponent<AliveController>().GetDamage(damage);
            Messenger.Broadcast(GameEvent.HIT);
            if (numberOfBreaks <= 0)
                Destroy(gameObject);
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