using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Weapon : MonoBehaviour
{
    public AmmoPack pack;
    public GameObject bullet;
    [Range(1, 200)] public float bulletSpeed = 20;
    [Range(1, 100)] public int damage = 15;
    
    [SerializeField] protected Transform shootPoint;
    public LayerMask ignoreMask;
    [SerializeField] protected AudioSource source;
    [SerializeField] protected AudioClip shoot;
    [SerializeField] protected AudioClip reload;
    [SerializeField] protected AudioClip fight;
    [SerializeField] private WeaponType type;
    
    [HideInInspector] public Transform lookPoint;
    [HideInInspector] public Animator anim;
    [HideInInspector] public bool opportunityToShoot;

    private bool opportunityToFight;
    private bool inMenu;

    void Awake()
    {
        GameController.PAUSE.AddListener(OnPause);
        anim = GetComponent<Animator>();
    }

    //каждое оружие имеет основной и альтернативный режим стрельбы
    public abstract void FirstShoot();
    public abstract void SecondShoot();
    public void Init(Transform point)
    {
        lookPoint = point;
    }


    private void Update()
    {
        if(!inMenu)
        {
            if (lookPoint != null)
            {
                shootPoint.LookAt(lookPoint);
            }
            FirstShoot();
            SecondShoot();
            Fight();
        }
    }
   
    public virtual void HideWeapon()
    {
        anim.SetTrigger("Hide");
    }

    public void SetFightOpportunityAsTrue() => opportunityToFight = true;
    public void SetFightOpportunityAsFalse() => opportunityToFight = false;
    public void PlayFightSound() => source.PlayOneShot(fight);
    
    private void Fight()
    {
        if(Input.GetKeyDown(KeyCode.F) && opportunityToFight)
        {
            anim.SetTrigger("Fight");
        }
    }

    private void OnHideWeapon()
    {
        GameController.WEAPON_ARE_HIDDEN.Invoke();
        opportunityToShoot = false;
    }
    private void OnWeaponReady()
    {
        GameController.WEAPON_READY.Invoke();
        anim.SetBool("Hide", false);
        opportunityToShoot = true;
    }

    private void OnPause(bool pause)
    {
        inMenu = pause;
    }
}

[System.Serializable]
public class AmmoPack
{
    [HideInInspector]
    public bool open;
    public int maxAmmo;
    public int currentAmmo;
    [Range(0.01f, 10)]
    public float bulletLifeTime = 3;
}
public enum WeaponType
{
    pistol = 0,
    shotgun = 1,
    rocketLauncher = 2,
    Minigun = 3
}
