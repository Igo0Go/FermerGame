using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AliveController : MonoBehaviour
{
    [Range(1,500)] public int maxHealth = 50;

    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0)
                Death();
            else if (_health > maxHealth)
                _health = maxHealth;
        }
    }
    private int _health;


    private void Start()
    {
        Health = maxHealth;
    }

    public virtual void GetDamage(int damage)
    {
        if(Health > 0)
            Health -= damage;
    }
    public abstract void Death();
}
