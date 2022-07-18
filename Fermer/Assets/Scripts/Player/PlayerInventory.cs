using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Weapon> weapons;

    [HideInInspector] public int currentWeapon = 0;

    private Transform lookpoint;

    private bool opportunityToChangeWeapon;

    private void Update()
    {
        FastCheckWeapon();
    }

    public void Setup()
    {
        GameController.WEAPON_ARE_HIDDEN.AddListener(SetWeapon);
        GameController.WEAPON_READY.AddListener(ReturnOpportunityToChangeWeapon);
        GameController.TAKE_BONUS_DAMAGE.AddListener(OnTakeBonusDamage);
        GameController.RETURN_TO_DEFAULT.AddListener(OnReturnToDefault);
        GameController.START_FINAL_LOADING.AddListener(HideAllWeapon);

        ConsoleEventCenter.Gun.Execute.AddListener(OnConsoleGunCommand);

        opportunityToChangeWeapon = false;

        foreach (var item in weapons)
        {
            item.pack.open = false;
        }
        CheckWeaponForChange(-1);
    }

    public void SetLookPoint(Transform lookPoint)
    {
        lookpoint = lookPoint;
    }
    public void CheckWeaponForChange(int number)
    {
        if(number == -1)
        {
            opportunityToChangeWeapon = false;
            if (currentWeapon > 0)
                weapons[currentWeapon].HideWeapon();
            currentWeapon = -1;
            GameController.WEAPON_ARE_CHANGED.Invoke(-1);
            SetWeapon();
        }
        if (currentWeapon != number && weapons[number].pack.open)
        {
            opportunityToChangeWeapon = false;
            if(currentWeapon >= 0)
                weapons[currentWeapon].HideWeapon();
            else
            {
                Invoke(nameof(SetWeapon), 1);
            }
            currentWeapon = number;
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
        opportunityToChangeWeapon = true;
        weapons[currentWeapon].Init(lookpoint);
        GameController.WEAPON_ARE_CHANGED.Invoke(currentWeapon);
        GameController.AMMO_ARE_CHANGED.Invoke(weapons[currentWeapon].pack.currentAmmo);
    }
    private void FastCheckWeapon()
    {
        if(opportunityToChangeWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CheckWeaponForChange(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
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

    private void OnConsoleGunCommand(int value)
    {
        value--;

        if(value >= 0 && value < 4)
        {
            Debug.Log("Оружие " + value + " добавлено");
            weapons[value].pack.open = true;
            weapons[value].pack.maxAmmo = 1000;
            weapons[value].pack.currentAmmo = 1000;
            CheckWeaponForChange(value);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Item"))
        {
            other.GetComponent<GameItem>().SetTarget(transform);
        }
    }
}
