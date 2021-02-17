using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Основной режим - бесконечные выстрелы клик-выстрел до первого препятствия.
 * Алтернативный - переключение в режим стрельбы снарядами, проходящими сквозь врагов, пока не столкнутся с препятствием или не истечёт время жизни снаряда
 */
public class Pistol : Weapon 
{
    [SerializeField] private AudioClip altShootSound;
    [SerializeField] private GameObject altBullet;

    
    private bool altShoot;

    private void Start()
    {
        opportunityToShoot = true;
        altShoot = false;
        anim.SetBool("AltShoot", altShoot);
    }

    public override void HideWeapon()
    {
        if(altShoot)
        {
            ChangeShootType();
        }
        base.HideWeapon();
    }

    public override void FirstShoot()
    {
        if(altShoot)
        {
            if (Input.GetButtonDown("Fire1") && opportunityToShoot && pack.currentAmmo > 0)
            {
                GameObject currentBullet = Instantiate(altBullet, shootPoint.position, shootPoint.rotation);
                currentBullet.GetComponent<Bullet>().Init(bulletSpeed / 2, pack.bulletLifeTime,
                    damage * 2 * PlayerBonusStat.bonusPack[BonusType.Damage], ignoreMask);
                source.PlayOneShot(altShootSound);
                anim.SetTrigger("Shoot");
                opportunityToShoot = false;
                pack.currentAmmo--;
                Messenger<int>.Broadcast(GameEvent.AMMO_ARE_CHANGED, pack.currentAmmo);
                if (pack.currentAmmo == 0)
                {
                    ChangeShootType();
                }
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && opportunityToShoot)
            {
                GameObject currentBullet = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
                currentBullet.GetComponent<Bullet>().Init(bulletSpeed, pack.bulletLifeTime,
                    damage * PlayerBonusStat.bonusPack[BonusType.Damage], ignoreMask);
                source.PlayOneShot(shoot);
                anim.SetTrigger("Shoot");
                opportunityToShoot = false;
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
