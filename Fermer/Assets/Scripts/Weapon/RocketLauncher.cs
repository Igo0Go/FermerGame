using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Основной - стрельба по прямой
 * Альтернативный - переключение в режим стрельбы с навесом
 */ 


public class RocketLauncher : Weapon
{
    [SerializeField] private GameObject altBullet;
    [SerializeField] private AudioClip altShootSound;
    [SerializeField] private Transform altShootPoint;

    private bool altShoot;

    public override void HideWeapon()
    {
        if (altShoot)
        {
            ChangeShootType();
        }
        base.HideWeapon();
    }

    public override void FirstShoot()
    {
        if (Input.GetButtonDown("Fire1") && opportunityToShoot && pack.currentAmmo > 0)
        {
            shootPoint.LookAt(lookPoint);
            GameObject currentBullet = Instantiate(altShoot ? altBullet : bullet, altShoot ? altShootPoint.position : shootPoint.position, shootPoint.rotation);
            currentBullet.GetComponent<Bullet>().Init(bulletSpeed, pack.bulletLifeTime,
                damage * PlayerBonusStat.bonusPack[BonusType.Damage], ignoreMask);
            source.PlayOneShot(altShootSound);
            anim.SetTrigger("Shoot");
            opportunityToShoot = false;
            pack.currentAmmo--;
            Messenger<int>.Broadcast(GameEvent.AMMO_ARE_CHANGED, pack.currentAmmo);
            if (altShoot && pack.currentAmmo == 0)
            {
                ChangeShootType();
            }
        }
    }

    public override void SecondShoot()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            ChangeShootType();
        }
    }

    private void ChangeShootType()
    {
        altShoot = !altShoot;
        anim.SetBool("AltShoot", altShoot);
        opportunityToShoot = false;
    }

    private void StartReloadSound()
    {
        source.PlayOneShot(reload);
    }
    private void ReturnOpportunityToShoot()
    {
        opportunityToShoot = true;
    }
}
