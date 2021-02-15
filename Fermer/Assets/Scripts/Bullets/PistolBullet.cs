﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBullet : Bullet //летит сквозь врагов
{
    public override void MoveBullet()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        transform.localScale += new Vector3(Time.deltaTime * speed / 5, 0, 0);
        Vector3 dir = transform.position - pos;
        if (Physics.Linecast(pos, transform.position, out RaycastHit hit, ~ignoreMask))
        {
            Hit(hit);
        }
        else if (Physics.BoxCast(transform.position - dir, transform.lossyScale / 2, dir, out hit, transform.rotation,
            dir.magnitude*2, ~ignoreMask))
        {
            AltHit(hit);
        }
        pos = transform.position;
    }

    public override void Hit(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Enemy"))
        {
            hit.collider.GetComponent<AliveController>().Death();
            Messenger.Broadcast(GameEvent.HIT);
            return;
        }
        else
        {
            GameObject obj = Instantiate(decal);
            obj.transform.position = hit.point;
            obj.transform.forward = hit.normal;
            obj.transform.localScale = transform.localScale;
            obj.GetComponent<Decal>().Init(1);
            Destroy(gameObject);
        }
    }

    private void AltHit(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Enemy"))
        {
            hit.collider.GetComponent<AliveController>().Death();
            Messenger.Broadcast(GameEvent.HIT);
            return;
        }
    }
}
