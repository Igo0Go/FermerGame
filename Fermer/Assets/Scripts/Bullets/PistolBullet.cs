using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PistolBullet : Bullet //летит сквозь врагов
{
    public override void MoveBullet()
    {
        base.MoveBullet();
        transform.localScale += new Vector3(Time.deltaTime * speed / 5, 0 ,0);
    }

    public override void Hit(RaycastHit hit)
    {
        if (hit.collider.tag.Equals("Enemy"))
        {
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
}
