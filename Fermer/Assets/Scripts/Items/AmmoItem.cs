using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : GameItem //пополнить количество патронов
{
    [SerializeField] private WeaponType type;
    [SerializeField, Range(1, 500), Tooltip("Количество патронов, которое нужно добавить")] private int ammoCount = 10;

    public override void Action()
    {
        int number = (int)type;

        PlayerInventory player = target.GetComponent<PlayerInventory>();
        player.weapons[number].pack.currentAmmo += ammoCount;
        if(player.weapons[number].pack.currentAmmo > player.weapons[number].pack.maxAmmo)
        {
            player.weapons[number].pack.currentAmmo = player.weapons[number].pack.maxAmmo;
        }
        if(player.currentWeapon == number)
            Messenger<int>.Broadcast(GameEvent.AMMO_ARE_CHANGED, player.weapons[number].pack.currentAmmo);
    }

    public override void SetTarget(Transform target)
    {
        int number = (int)type;

        PlayerInventory player = target.GetComponent<PlayerInventory>();
        if(player.weapons[number].pack.open)
        {
            if(player.weapons[number].pack.currentAmmo < player.weapons[number].pack.maxAmmo)
            {
                this.target = target;
            }
        }
    }
}
