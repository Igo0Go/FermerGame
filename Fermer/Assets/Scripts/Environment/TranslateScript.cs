using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateScript : MonoBehaviour
{
    [SerializeField, Range(1, 10)]private float speed = 1;
    [SerializeField, Range(0, 10)] private float delay = 0;
    [SerializeField] private Vector3 offsetPos;
    [SerializeField, Space(10)] private bool debug;

    private Vector3 startPos;
    private Vector3 targetPos;
    Vector3 dir;
    private int multiplicator = 1;
    private bool move;
    private float currentSpeed;

    private float DistanceToTarget => Vector3.Distance(transform.position, targetPos);

    void Start()
    {
        startPos = transform.position;
    }
    void Update()
    {
        if (move)
            MoveToTarget();
    }

    public void ToDefaultPos()
    {
        targetPos = startPos;
        dir = targetPos - transform.position;
        currentSpeed = 10;
        multiplicator = 1;
        move = true;
    }

    public void ChangePosition()
    {
        Invoke("SetTargetPos", delay);
    }

    private void SetTargetPos()
    {
        currentSpeed = speed;
        targetPos = transform.position + offsetPos * multiplicator;
        multiplicator *= -1;
        dir = targetPos - transform.position;
        move = true;
    }
    private void MoveToTarget()
    {
        if(DistanceToTarget > speed * Time.deltaTime * 3)
        {
            transform.position += dir.normalized * speed * Time.deltaTime;
        }
        else
        {
            transform.position = targetPos;
            move = false;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(debug)
        {
            Gizmos.color = Color.cyan;
                targetPos = transform.position + offsetPos;

            //задняя грань

            Vector3 start = targetPos
                - transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                - transform.forward * transform.lossyScale.z / 2;
            Vector3 end = targetPos
                + transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                - transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = targetPos
                + transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                - transform.forward * transform.lossyScale.z / 2;
            end = targetPos
                + transform.right * transform.lossyScale.x / 2
                + transform.up * transform.lossyScale.y / 2
                - transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = targetPos
               + transform.right * transform.lossyScale.x / 2
               + transform.up * transform.lossyScale.y / 2
               - transform.forward * transform.lossyScale.z / 2;
            end = targetPos
                - transform.right * transform.lossyScale.x / 2
                + transform.up * transform.lossyScale.y / 2
                - transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = targetPos
               - transform.right * transform.lossyScale.x / 2
               + transform.up * transform.lossyScale.y / 2
               - transform.forward * transform.lossyScale.z / 2;
            end = targetPos
                - transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                - transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            //боковые рёбра

            start = targetPos
               - transform.right * transform.lossyScale.x / 2
               - transform.up * transform.lossyScale.y / 2
               - transform.forward * transform.lossyScale.z / 2;
            end = targetPos
                - transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = targetPos
               + transform.right * transform.lossyScale.x / 2
               - transform.up * transform.lossyScale.y / 2
               - transform.forward * transform.lossyScale.z / 2;
            end = targetPos
                + transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = targetPos
               + transform.right * transform.lossyScale.x / 2
               + transform.up * transform.lossyScale.y / 2
               - transform.forward * transform.lossyScale.z / 2;
            end = targetPos
                + transform.right * transform.lossyScale.x / 2
                + transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = targetPos
               - transform.right * transform.lossyScale.x / 2
               + transform.up * transform.lossyScale.y / 2
               - transform.forward * transform.lossyScale.z / 2;
            end = targetPos
                - transform.right * transform.lossyScale.x / 2
                + transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            //передняя грань

            start = targetPos
                  - transform.right * transform.lossyScale.x / 2
                  - transform.up * transform.lossyScale.y / 2
                  + transform.forward * transform.lossyScale.z / 2;
            end = targetPos
                + transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = targetPos
                + transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            end = targetPos
                + transform.right * transform.lossyScale.x / 2
                + transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = targetPos
               + transform.right * transform.lossyScale.x / 2
               + transform.up * transform.lossyScale.y / 2
               + transform.forward * transform.lossyScale.z / 2;
            end = targetPos
                - transform.right * transform.lossyScale.x / 2
                + transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = targetPos
               - transform.right * transform.lossyScale.x / 2
               + transform.up * transform.lossyScale.y / 2
               + transform.forward * transform.lossyScale.z / 2;
            end = targetPos
                - transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);
        }
    }
#endif
}
