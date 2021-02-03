using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineItem : GameItem //аптечка
{
    [SerializeField, Range(1,100)] private int hp = 25;

    public override void SetTarget(Transform target)
    {
        AliveController alive = target.GetComponent<AliveController>();
        if (alive.Health < alive.maxHealth)
            base.SetTarget(target);
    }

    public override void Action()
    {
        PlayerCharacter alive = target.GetComponent<PlayerCharacter>();
        alive.RestoreHealth(hp);
    }
}
