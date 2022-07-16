﻿using UnityEngine;

public class PistolBullet : Bullet //летит сквозь врагов
{
    public override void MoveBullet()
    {
        transform.position += speed * Time.deltaTime * transform.forward;
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
            hit.collider.GetComponent<AliveController>().GetDamage(damage);
            GameController.HIT.Invoke();
            return;
        }
        else if (hit.collider.CompareTag("Bullet"))
        {
            if (hit.collider.TryGetComponent<TargetTrackerBullet>(out TargetTrackerBullet bullet))
            {
                bullet.Explosion();
                GameController.HIT.Invoke();
                Destroy(gameObject);
                return;
            }
        }
        else if(playerReact && hit.collider.CompareTag("Player"))
        {
            hit.collider.GetComponent<PlayerCharacter>().OnTakeDamageFromDirection(transform.position);
            hit.collider.GetComponent<AliveController>().GetDamage(damage);
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
            obj.transform.localScale = transform.localScale;
            Destroy(gameObject);
        }
    }

    private void AltHit(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Enemy"))
        {
            hit.collider.GetComponent<AliveController>().GetDamage(90);
            GameController.HIT.Invoke();
            return;
        }
        else if (hit.collider.CompareTag("Bullet"))
        {
            if (hit.collider.TryGetComponent(out TargetTrackerBullet bullet))
            {
                bullet.Explosion();
                GameController.HIT.Invoke();
                return;
            }
        }
        else if (playerReact && hit.collider.CompareTag("Player"))
        {
            hit.collider.GetComponent<PlayerCharacter>().OnTakeDamageFromDirection(transform.position);
            hit.collider.GetComponent<AliveController>().GetDamage(damage);
        }
        else if (hit.collider.CompareTag("InteractiveBox"))
        {
            hit.collider.GetComponent<InteractiveBox>().OnFightAction();
            GameController.HIT.Invoke();
        }
    }
}
