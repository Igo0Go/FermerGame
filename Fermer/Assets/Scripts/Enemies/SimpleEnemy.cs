using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    MoveToTarget,
    Attack,
    Recoil,
    Death
}

[RequireComponent(typeof(Rigidbody))]
public abstract class Enemy : AliveController
{
    [SerializeField, Range(1,100), Tooltip("Количество очков, получаемое за победу над врагом")] protected int scoreForWin = 1;
    [SerializeField, Range(1, 100)] protected int damage = 10;
    [SerializeField] private List<GameObject> lootPrefabs;
    [SerializeField] protected GameObject postDeadDecal;
    [SerializeField] protected GameObject afterFightLoot;
    
    private Rigidbody rb;

    private void Start()
    {
        Health = maxHealth;
        rb = GetComponent<Rigidbody>();
        ReturnRB();
    }

    protected virtual void OnFightAction()
    {
        GameController.ENEMY_HIT.Invoke(scoreForWin);
        Vector3 dir = new Vector3(Random.Range(-0.05f, 0.05f), 2, Random.Range(-0.05f, 0.05f));
        Instantiate(afterFightLoot, transform.position + dir, Quaternion.identity).GetComponent<Rigidbody>()
            .AddForce(dir, ForceMode.Impulse);
        Instantiate(postDeadDecal, transform.position, Quaternion.identity).GetComponent<Decal>().Init(2);
        GameController.ENEMY_DEAD.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            GameController.HIT.Invoke();
            GetDamage(other.GetComponent<Bullet>().damage);
        }
        else if(other.CompareTag("Fire"))
        {
            ExplosionZone zone = other.GetComponent<ExplosionZone>();
            if(zone != null)
            {
                if(rb == null)
                {
                    rb = GetComponent<Rigidbody>();
                }
                rb.useGravity = true;
                rb.isKinematic = false;

                Vector3 dir = other.transform.position - (transform.position + Vector3.up);
                rb.AddForce(dir.normalized * zone.force, ForceMode.Impulse);
                Invoke(nameof(ReturnRB), 1);
                GetDamage(zone.damage);
            }
        }
        else if(other.CompareTag("Turret"))
        {
            other.GetComponent<Turret>().AddTarget(transform);
        }
        else if(other.CompareTag("DeadZone"))
        {
            Death();
        }
        else if (other.CompareTag("Blade"))
        {
            GameController.HIT.Invoke();
            OnFightAction();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Turret"))
        {
            other.GetComponent<Turret>().RemoveTarget(transform);
        }
    }

    public override void Death()
    {
        GameController.ENEMY_HIT.Invoke(scoreForWin);
        int cycleCount = Random.Range(0, 3);

        if (cycleCount > lootPrefabs.Count)
            cycleCount = lootPrefabs.Count;

        while (cycleCount > 0)
        {
            int number = Random.Range(0, lootPrefabs.Count);
            Vector3 dir = new Vector3(Random.Range(-0.05f, 0.05f), 2, Random.Range(-0.05f, 0.05f));
            Instantiate(lootPrefabs[number], transform.position + dir, Quaternion.identity).GetComponent<Rigidbody>().AddForce(dir, ForceMode.Impulse);
            lootPrefabs.Remove(lootPrefabs[number]);
            cycleCount--;
        }
        Instantiate(postDeadDecal, transform.position, Quaternion.identity).GetComponent<Decal>().Init(2);
        GameController.ENEMY_DEAD.Invoke();
        Destroy(gameObject);
    }
    private void ReturnRB()
    {
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
    }
}

public class SimpleEnemy : Enemy
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField, Tooltip("Расстояние, на котором носитель скрипта будет реагировать на препятствие (длина луча)")] protected float obstacleRange = 5.0f;
    [SerializeField, Tooltip("Слои, которые не будут считаться препятствиями")] protected LayerMask ignoreMask;
    [SerializeField, Tooltip("Место появления снаряда")] protected Transform shootPoint;

    protected bool recoil = false;

    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
        RayShoot();
    }

    public virtual void RayShoot()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.SphereCast(ray, 2, out RaycastHit hit))
        {

            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.GetComponent<PlayerCharacter>())
            {
                if (!recoil)
                {
                    shootPoint.LookAt(hitObject.transform.position + Vector3.up);
                    GameObject currentBullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation) as GameObject;
                    currentBullet.GetComponent<Bullet>().Init(50, 5, damage, ignoreMask);
                    recoil = true;
                    Invoke(nameof(StopRecoil), 1);
                }
            }
            else if (hit.distance < obstacleRange)
            {
                float angle = Random.Range(-110, 110);
                transform.Rotate(0, angle, 0);
            }
        }
    }

    /// <summary>
    /// Вызывается с помощью Invoke
    /// </summary>
    private void StopRecoil() => recoil = false;
}
