using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NLO : Enemy //Нло-вертушка с крутящимися лезвиями
{
    [SerializeField, Tooltip("Ось вращения")] private Transform DeadZoneObjectsCenter;
    [SerializeField, Range(1,5), Tooltip("На какое расстяние подходить к цели для атаки")] private float attakDistance = 3;
    [SerializeField, Range(1, 360)] private float rotationSpeed = 1;

    private NavMeshAgent agent;
    private Transform target;


    void Start()
    {
        Health = maxHealth;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        Move();
        RotateDeadZones();
    }

    private void Move()
    {
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            if (dir.magnitude <= attakDistance)
            {
                dir.y = 0;
                transform.forward = dir.normalized;
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                agent.destination = target.position;
            }
        }
        else if (!agent.isStopped)
        {
            agent.isStopped = true;
        }
    }
    private void RotateDeadZones()
    {
        DeadZoneObjectsCenter.Rotate(transform.up, rotationSpeed * Time.deltaTime);
    }
}
