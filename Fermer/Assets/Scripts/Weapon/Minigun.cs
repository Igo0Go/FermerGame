using UnityEngine;

/*
 * Основной режим - стрельба пока зажата кнопка мыши
 * Ставим турель под ноги
 */
public class Minigun : Weapon
{
    [SerializeField, Range(0.001f, 1)] private float recoilTime = 0.5f;
    [SerializeField, Range(0, 1)] private float angle = 0.2f;
    [SerializeField] private GameObject turretPrefab;

    public override void FirstShoot()
    {
        if (Input.GetButton("Fire1") && pack.currentAmmo > 0)
        {
            if (opportunityToShoot)
            {
                GameObject currentBullet = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
                currentBullet.transform.forward = (currentBullet.transform.forward + currentBullet.transform.up * Random.Range(-angle, angle) +
                    currentBullet.transform.right * Random.Range(-angle, angle)).normalized;
                currentBullet.GetComponent<Bullet>().Init(bulletSpeed, pack.bulletLifeTime,
                    damage * PlayerBonusStat.bonusPack[BonusType.Damage], ignoreMask);
                source.PlayOneShot(shoot);
                anim.SetBool("Shoot", true);
                opportunityToShoot = false;
                pack.currentAmmo--;
                GameController.AMMO_ARE_CHANGED.Invoke(pack.currentAmmo);
                Invoke(nameof(ReturnOpportunityToShoot), recoilTime);
            }
            else
            {
                anim.SetBool("Shoot", false);
            }
        }
        else
        {
            anim.SetBool("Shoot", false);
        }
    }

    public override void SecondShoot()
    {
        if (Input.GetButton("Fire2") && pack.currentAmmo >= 0)
        {
            opportunityToShoot = false;
            anim.SetBool("InstanceTurret", true);
            pack.open = false;
        }
    }

    private void OnTurretInstance()
    {
        anim.SetBool("InstanceTurret", false);
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5, ~ignoreMask))
        {
            GameObject turret = Instantiate(turretPrefab);
            turret.transform.position = hit.point + hit.normal * 0.15f;
            turret.transform.up = hit.normal;
            turret.GetComponent<Turret>().Init(this);
            GameController.RETURN_TO_DEFAULT.Invoke();
        }
    }
    private void ReturnOpportunityToShoot()
    {
        opportunityToShoot = true;
    }
}
