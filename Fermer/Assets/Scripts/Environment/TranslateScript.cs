using System.Collections;
using UnityEngine;

public class TranslateScript : MonoBehaviour
{
    [SerializeField, Range(1, 10)]private float moveTime = 1;
    [SerializeField, Range(0, 10)] private float delay = 0;
    [SerializeField] private Vector3 offsetPos;
    [SerializeField, Space(10)] private bool debug;
    public bool loopMove = false;

    private bool toEndPoint;
    private float t = 0;

    private Vector3 startPos;
    private Vector3 endPoint;

    void Start()
    {
        startPos = transform.position;
        endPoint = startPos + offsetPos;
        toEndPoint = false;
        t = 0;
    }

    public void SetMovingTime(float newSpeed)
    {
        if(newSpeed < 0)
        {
            newSpeed *= -1;
        }
        moveTime = newSpeed;
    }

    public void ToDefaultPos()
    {
        loopMove = false;
        toEndPoint = false;
        StopAllCoroutines();
        StartCoroutine(MoveToStartCoroutine());
    }

    public void ChangePosition()
    {
        StopAllCoroutines();
        toEndPoint = !toEndPoint;
        if(toEndPoint)
        {
            StartCoroutine(MoveToEndCoroutine());
        }
        else
        {
            StartCoroutine(MoveToStartCoroutine());
        }
    }

    private IEnumerator MoveToEndCoroutine()
    {
        yield return new WaitForSeconds(delay);

        while (t<1)
        {
            t += Time.deltaTime/moveTime;
            transform.position = Vector3.Lerp(startPos, endPoint, t);
            yield return null;
        }

        transform.position = endPoint;
        t = 1;

        if(loopMove)
        {
            ChangePosition();
        }
    }
    private IEnumerator MoveToStartCoroutine()
    {
        yield return new WaitForSeconds(delay);
        while (t > 0)
        {
            t -= Time.deltaTime/moveTime;
            transform.position = Vector3.Lerp(startPos, endPoint, t);
            yield return null;
        }

        transform.position = startPos;
        t = 0;

        if (loopMove)
        {
            ChangePosition();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(debug)
        {
            Gizmos.color = Color.cyan;
                endPoint = transform.position + offsetPos;

            //задняя грань

            Vector3 start = endPoint
                - transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                - transform.forward * transform.lossyScale.z / 2;
            Vector3 end = endPoint
                + transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                - transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = endPoint
                + transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                - transform.forward * transform.lossyScale.z / 2;
            end = endPoint
                + transform.right * transform.lossyScale.x / 2
                + transform.up * transform.lossyScale.y / 2
                - transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = endPoint
               + transform.right * transform.lossyScale.x / 2
               + transform.up * transform.lossyScale.y / 2
               - transform.forward * transform.lossyScale.z / 2;
            end = endPoint
                - transform.right * transform.lossyScale.x / 2
                + transform.up * transform.lossyScale.y / 2
                - transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = endPoint
               - transform.right * transform.lossyScale.x / 2
               + transform.up * transform.lossyScale.y / 2
               - transform.forward * transform.lossyScale.z / 2;
            end = endPoint
                - transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                - transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            //боковые рёбра

            start = endPoint
               - transform.right * transform.lossyScale.x / 2
               - transform.up * transform.lossyScale.y / 2
               - transform.forward * transform.lossyScale.z / 2;
            end = endPoint
                - transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = endPoint
               + transform.right * transform.lossyScale.x / 2
               - transform.up * transform.lossyScale.y / 2
               - transform.forward * transform.lossyScale.z / 2;
            end = endPoint
                + transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = endPoint
               + transform.right * transform.lossyScale.x / 2
               + transform.up * transform.lossyScale.y / 2
               - transform.forward * transform.lossyScale.z / 2;
            end = endPoint
                + transform.right * transform.lossyScale.x / 2
                + transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = endPoint
               - transform.right * transform.lossyScale.x / 2
               + transform.up * transform.lossyScale.y / 2
               - transform.forward * transform.lossyScale.z / 2;
            end = endPoint
                - transform.right * transform.lossyScale.x / 2
                + transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            //передняя грань

            start = endPoint
                  - transform.right * transform.lossyScale.x / 2
                  - transform.up * transform.lossyScale.y / 2
                  + transform.forward * transform.lossyScale.z / 2;
            end = endPoint
                + transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = endPoint
                + transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            end = endPoint
                + transform.right * transform.lossyScale.x / 2
                + transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = endPoint
               + transform.right * transform.lossyScale.x / 2
               + transform.up * transform.lossyScale.y / 2
               + transform.forward * transform.lossyScale.z / 2;
            end = endPoint
                - transform.right * transform.lossyScale.x / 2
                + transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);

            start = endPoint
               - transform.right * transform.lossyScale.x / 2
               + transform.up * transform.lossyScale.y / 2
               + transform.forward * transform.lossyScale.z / 2;
            end = endPoint
                - transform.right * transform.lossyScale.x / 2
                - transform.up * transform.lossyScale.y / 2
                + transform.forward * transform.lossyScale.z / 2;
            Gizmos.DrawLine(start, end);
        }
    }
#endif
}
