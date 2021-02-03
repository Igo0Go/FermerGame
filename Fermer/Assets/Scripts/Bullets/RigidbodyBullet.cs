using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(Rigidbody))]
public class RigidbodyBullet : Bullet //снаряды, имеющие вес
{
    [SerializeField, Tooltip("Усилить запуск. При значении false можно использовать, как вертикальную бомбу.")] private bool AddForce = true;

    private const float forceOfFiring = 25; //для корректировки скорости полёта снаряда по прежнему используется поле speed. Данное поле - просто множитель.

    private Action<Collision> reactType;
    
    public override void Init(float speed, float lifetime, int damage, LayerMask ignoreMask)
    {
        if (playerReact)
        {
            reactType = FoolReact;
        }
        else
        {
            reactType = ReactWithoutPlayer;
        }

        if (AddForce)
            GetComponent<Rigidbody>().AddForce(transform.forward * speed * forceOfFiring);
        this.damage = damage;
        Destroy(gameObject, lifetime);
    }
    public override void MoveBullet()
    {

    }

    private void FoolReact(Collision collision)
    {
        GameObject obj = Instantiate(decal);
        obj.transform.position = transform.position;
        obj.GetComponent<Decal>().Init(3);
    }
    private void ReactWithoutPlayer(Collision collision)
    {
        if(!collision.gameObject.tag.Equals("Player"))
        {
            GameObject obj = Instantiate(decal);
            obj.transform.position = transform.position;
            obj.GetComponent<Decal>().Init(3);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        reactType(collision);
    }
}
