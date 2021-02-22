using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class Goose : Enemy //Гусь с ракетной установкой (по совместительству ваш худший кошмар)
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField, Tooltip("Слои, на котороые гусь не будет реагировтаь")] private LayerMask ignoreMask;
    [SerializeField, Tooltip("Цель, которую гусь будет атаковать")] private Transform target;
    [SerializeField, Tooltip("Расстояние, с которого гусь может остановиться и стрелять"), Range(1, 50)] private float attakDistance = 15;
    [SerializeField, Range(0.01f, 1), Tooltip("Время между выстрелами")] float recoilTime = 0.4f;
    [SerializeField, Range(1, 50), Tooltip("Скорость запуска снаряда")] float bulletSpeed = 10;
    [SerializeField, Tooltip("Точки, в которых бдут по очереди появляться снаряды при выстреле")] List<Transform> shootPoints;
    [SerializeField, Range(0, 3)] private float timeBetweenShootActions = 1f;

    private NavMeshAgent agent;
    private EnemyState state;
    private Animator anim;
    private int opportunityToShoot;
    private int currentShootPoint;

    public void Init(Transform target)
    {
        this.target = target;
        agent.destination = target.position;
        agent.isStopped = false;
    }

    public override void GetDamage(int damage)
    {
        if(Health- damage > 0)
        {
            anim.SetTrigger("Damage");
        }
        base.GetDamage(damage);
    }
    protected override void OnFightAction()
    {
        Messenger<int>.Broadcast(GameEvent.ENEMY_HIT, scoreForWin);
        if (Health - 50 > 0)
        {
            GetDamage(50);
        }
        else
        {
            Vector3 dir = new Vector3(Random.Range(-0.05f, 0.05f), 2, Random.Range(-0.05f, 0.05f));
            Instantiate(afterFightLoot, transform.position + dir, Quaternion.identity).GetComponent<Rigidbody>()
                .AddForce(dir, ForceMode.Impulse);
            Instantiate(postDeadDecal, transform.position, Quaternion.identity).GetComponent<Decal>().Init(2);
            Messenger.Broadcast(GameEvent.ENEMY_DEAD);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Health = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        if(target != null)
            Init(target);
        state = EnemyState.MoveToTarget;
        anim = GetComponent<Animator>();
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if(obj != null)
        {
            Init(obj.transform);
        }
    }
    private void Update()
    {
        switch (state)
        {
            case EnemyState.MoveToTarget:
                Move();
                break;
            case EnemyState.Attack:
                Shoot();
                break;
            default:
                break;
        }
    }

    private void Move()
    {
        if(target != null)
        {
            Vector3 dir = target.position - transform.position;
            if(dir.magnitude <= attakDistance)
            {
                dir.y = 0;
                transform.forward = dir.normalized;
                agent.isStopped = true;
                if(opportunityToShoot == 0)
                {
                    opportunityToShoot = 1;
                    currentShootPoint = 0;
                    anim.SetBool("Move", false);
                    state = EnemyState.Attack;
                }
            }
            else
            {
                anim.SetBool("Move", true);
                agent.isStopped = false;
                agent.destination = target.position;
            }
        }
        else if(!agent.isStopped)
        {
            agent.isStopped = true;
        }
    }
    private void Shoot()
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        transform.forward = dir.normalized;
        
        if (opportunityToShoot == 1)
        {
            ShootAction();
            opportunityToShoot = 2;
        }
    }

    private void ShootAction()
    {
        shootPoints[currentShootPoint].LookAt(target);
        GameObject currentBullet = Instantiate(bulletPrefab) as GameObject;
        currentBullet.transform.position = shootPoints[currentShootPoint].position;
        currentBullet.transform.rotation = transform.rotation;
        Bullet bulletScript = currentBullet.GetComponent<Bullet>();
        bulletScript.Init(bulletSpeed, 5, damage, ignoreMask);
        if (bulletScript is TargetTrackerBullet)
        {
            ((TargetTrackerBullet)bulletScript).SetTarget(target);
        }
        Invoke("NextShootAction", timeBetweenShootActions);
        state = EnemyState.Recoil;
    }

    private void NextShootAction()
    {
        anim.SetInteger("Shoot", currentShootPoint + 1);
    }


    #region Обработчики событий анимации
    private void StopShoot()
    {
        currentShootPoint++;
        if (currentShootPoint > shootPoints.Count - 1)
        {
            opportunityToShoot = -1;
            Invoke("ReturnOpportunityToShoot", recoilTime);
            state = EnemyState.MoveToTarget;
        }
        else
        {
                Invoke("ShootAgain", recoilTime);
        }
        anim.SetInteger("Shoot", 0);
    }
    private void ShootAgain()
    {
        opportunityToShoot = 1;
        state = EnemyState.Attack;
    }
    private void ReturnOpportunityToShoot() => opportunityToShoot = 0;
    #endregion
}
