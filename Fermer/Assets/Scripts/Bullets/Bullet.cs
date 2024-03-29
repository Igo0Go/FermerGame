﻿using UnityEngine;
using System;

public class Bullet : MonoBehaviour //уничтожается при первом столкновении
{
    [Tooltip("След от выстрела (префаб)")] public GameObject decal;

    [HideInInspector] public float speed;
    [HideInInspector] public int damage;
    [SerializeField, Tooltip("Наносит урон игроку (для врагов)")] protected bool playerReact = false;
    [SerializeField] protected LayerMask ignoreMask;
    
    protected Vector3 pos;

    private Action<RaycastHit> hitAction;

    public virtual void Init(float speed, float lifetime, int damage, LayerMask ignoreMask)
    {
        this.speed = speed;
        this.damage = damage;
        this.ignoreMask = ignoreMask;
        pos = transform.position;
        Destroy(gameObject, lifetime);
        if (playerReact)
            hitAction = ReactFool;
        else
            hitAction = ReactWithoutPlayer;
    }

    private void Update()
    {
        MoveBullet();
    }

    public virtual void MoveBullet()
    {
        transform.position += speed * Time.deltaTime * transform.forward;
        if(Physics.Linecast(pos, transform.position, out RaycastHit hit ,~ignoreMask))
        {
            Hit(hit);
        }
        pos = transform.position;
    }

    public virtual void Hit(RaycastHit hit)
    {
        hitAction(hit);
    }

    private void ReactFool(RaycastHit hit)
    {
        GameObject obj = Instantiate(decal);
        obj.transform.position = hit.point;
        obj.transform.forward = hit.normal;
        obj.transform.parent = hit.collider.transform;

        if (hit.collider.CompareTag("Enemy"))
        {
            hit.collider.GetComponent<AliveController>().GetDamage(damage);
            GameController.HIT.Invoke();
        }
        else if(hit.collider.CompareTag("Player"))
        {
            hit.collider.GetComponent<PlayerCharacter>().OnTakeDamageFromDirection(transform.position);
            hit.collider.GetComponent<AliveController>().GetDamage(damage);
        }
        else if(hit.collider.CompareTag("Bullet"))
        {
            if(hit.collider.TryGetComponent(out TargetTrackerBullet bullet))
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

        Destroy(gameObject);
    }

    private void ReactWithoutPlayer(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Enemy"))
        {
            hit.collider.GetComponent<AliveController>().GetDamage(damage);
            GameController.HIT.Invoke();
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

        GameObject obj = Instantiate(decal);
        obj.transform.position = hit.point;
        obj.transform.forward = hit.normal;
        obj.transform.parent = hit.collider.transform;

        Destroy(gameObject);
    }
}
