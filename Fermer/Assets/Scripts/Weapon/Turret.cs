using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject postDeadDecal;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform rotor;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip shootClip;
    [SerializeField, Range(0.001f, 1)] private float recoilTime = 0.5f;

    private GameObject bullet;
    private List<Transform> targets;
    private float bulletSpeed;
    private float bulletLifeTime;
    private int damage;
    private int currentAmmo;
    private LayerMask ignoreMask;
    private sbyte opportunityToShoot;

    void Update()
    {
        Check();
    }

    public void Init(Weapon weapon)
    {
        bullet = weapon.bullet;
        bulletSpeed = weapon.bulletSpeed;
        bulletLifeTime = weapon.pack.bulletLifeTime;
        damage = weapon.damage;
        currentAmmo = weapon.pack.currentAmmo;
        ignoreMask = weapon.ignoreMask;
        opportunityToShoot = 1;
        targets = new List<Transform>();
    }
    public void AddTarget(Transform target)
    {
        if(!targets.Contains(target))
            targets.Add(target);
    }
    public void RemoveTarget(Transform target)
    {
        if (targets.Contains(target))
            targets.Remove(target);
    }

    private void Check()
    {
        if(opportunityToShoot == 1 && currentAmmo > 0)
        {
            ShootInNear(FindNearlyTarget());
        }
        else if(opportunityToShoot >= 0 && currentAmmo <= 0)
        {
            opportunityToShoot = -1;
            Invoke("Selfdestruction", 2);
        }
    }
    private void ShootInNear(Transform target)
    {
        if(target != null)
        {
            rotor.LookAt(target.position + Vector3.up);
            GameObject currentBullet = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
            currentBullet.GetComponent<Bullet>().Init(bulletSpeed, bulletLifeTime,
                damage * PlayerBonusStat.bonusPack[BonusType.Damage], ignoreMask);
            source.PlayOneShot(shootClip);
            opportunityToShoot = 0;
            currentAmmo--;
            Invoke("ReturnOpportunityToShoot", recoilTime);
        }
    }
    private Transform FindNearlyTarget()
    {
        Transform result = null;
        for (int i = 0; i < targets.Count; i++)
        {
            if(targets[i] == null)
            {
                targets.Remove(targets[i]);
                i--;
            }
            else
            {
                if (result == null) result = targets[i];
                else
                {
                    Vector3 oldDir = transform.position - result.position;
                    Vector3 newDir = transform.position - targets[i].position;
                    if (Physics.Raycast(transform.position, newDir, out RaycastHit hit, newDir.magnitude *1.5f) && hit.transform == targets[i])
                    {
                        if (newDir.magnitude < oldDir.magnitude)
                            result = targets[i];
                    }
                }
            }
        }

        return result;
    }
    private void ReturnOpportunityToShoot()
    {
        opportunityToShoot = 1;
    }

    private void Selfdestruction()
    {
        Instantiate(postDeadDecal, transform.position, Quaternion.identity).GetComponent<Decal>().Init(2);
        Destroy(gameObject);
    }
}

