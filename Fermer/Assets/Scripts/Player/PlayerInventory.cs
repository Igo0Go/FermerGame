using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Weapon> weapons;

    [HideInInspector] public int currentWeapon = 0;

    private Transform lookpoint;
   // private bool opportunityToChangeWeapon = true;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.WEAPON_ARE_HIDDEN, SetWeapon);
        Messenger.AddListener(GameEvent.WEAPON_READY, ReturnOpportunityToChangeWeapon);
        Messenger<int>.AddListener(GameEvent.TAKE_BONUS_DAMAGE, OnTakeBonusDamage);
        Messenger.AddListener(GameEvent.RETURN_TO_DEFAULT, OnReturnToDefault);
        Messenger.AddListener(GameEvent.START_FINAL_LOADING, HideAllWeapon);

      //  Messenger.AddListener(GameEvent.EXIT_LEVEL, OnDestroy);
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.WEAPON_ARE_HIDDEN, SetWeapon);
        Messenger.RemoveListener(GameEvent.WEAPON_READY, ReturnOpportunityToChangeWeapon);
        Messenger<int>.RemoveListener(GameEvent.TAKE_BONUS_DAMAGE, OnTakeBonusDamage);
        Messenger.RemoveListener(GameEvent.RETURN_TO_DEFAULT, OnReturnToDefault);
        Messenger.RemoveListener(GameEvent.START_FINAL_LOADING, HideAllWeapon);

        //   Messenger.RemoveListener(GameEvent.EXIT_LEVEL, OnDestroy);
    }

    private void Update()
    {
        FastCheckWeapon();
    }

    public void Setup()
    {
        foreach (var item in weapons)
        {
            item.pack.open = false;
        }
        CheckWeaponForChange(-1);
        //opportunityToChangeWeapon = true;
    }

    public void SetLookPoint(Transform lookPoint)
    {
        lookpoint = lookPoint;
    }
    public void CheckWeaponForChange(int number)
    {
        if(number == -1)
        {
            if(currentWeapon > 0)
                weapons[currentWeapon].HideWeapon();
            currentWeapon = -1;
            Messenger<int>.Broadcast(GameEvent.WEAPON_ARE_CHANGED, -1);
            SetWeapon();
        }
        if (currentWeapon != number && weapons[number].pack.open)
        {
            if(currentWeapon >= 0)
                weapons[currentWeapon].HideWeapon();
            else
            {
                Invoke("SetWeapon", 1);
            }
            currentWeapon = number;
            //opportunityToChangeWeapon = false;
        }
    }

    private void SetWeapon()
    {
        foreach (var item in weapons)
        {
            item.gameObject.SetActive(false);
        }
        if(currentWeapon >= 0)
        {
            weapons[currentWeapon].gameObject.SetActive(true);
        }
    }
    private void ReturnOpportunityToChangeWeapon()
    {
        //opportunityToChangeWeapon = true;
        weapons[currentWeapon].Init(lookpoint);
        Messenger<int>.Broadcast(GameEvent.WEAPON_ARE_CHANGED, currentWeapon);
        Messenger<int>.Broadcast(GameEvent.AMMO_ARE_CHANGED, weapons[currentWeapon].pack.currentAmmo);
    }
    private void FastCheckWeapon()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            CheckWeaponForChange(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            CheckWeaponForChange(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CheckWeaponForChange(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CheckWeaponForChange(3);
        }
    }

    private void OnReturnToDefault()
    {
        if(weapons[0].pack.open)
            CheckWeaponForChange(0);
        else
            CheckWeaponForChange(-1);
    }
    private void OnTakeBonusDamage(int value)
    {
        PlayerBonusStat.bonusPack[BonusType.Damage] = value;
    }
    private void HideAllWeapon() => CheckWeaponForChange(-1);

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Item"))
        {
            other.GetComponent<GameItem>().SetTarget(transform);
        }
    }
}
