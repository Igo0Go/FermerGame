using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : GameItem //подбираемое оружие
{
    [SerializeField] private WeaponType type;
    [SerializeField, Range(1, 500)] private int maxAmmoCount = 10;
    
    public override void Action()
    {
        int number = (int)type;

        PlayerInventory player = target.GetComponent<PlayerInventory>();
        player.weapons[number].pack.open = true;
        player.weapons[number].pack.maxAmmo = maxAmmoCount;
        player.weapons[number].pack.currentAmmo = maxAmmoCount;
        player.CheckWeaponForChange(number);
    }

    public override void SetTarget(Transform target)
    {
        if(type == WeaponType.Minigun)
        {
            PlayerInventory player = target.GetComponent<PlayerInventory>();
            if(!player.weapons[(int)type].pack.open)
            {
                base.SetTarget(target);
            }
            return;
        }
        base.SetTarget(target);
    }
}
