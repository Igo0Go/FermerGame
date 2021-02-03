using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kurojabel : SimpleEnemy //дережабль в виде курицы, который посылает сверху бомбы-яйца
{
    [SerializeField, Range(1,5)] private float recoilTyme;


    private void Start()
    {
        Health = maxHealth;
    }

    public override void RayShoot()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, 0.75f, out hit))
        {
            if (hit.distance < obstacleRange)
            {
                float angle = Random.Range(-110, 110);
                transform.Rotate(0, angle, 0);
            }

            if (!recoil)
            {
                GameObject currentBullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity) as GameObject;
                currentBullet.GetComponent<Bullet>().Init(50, 5, 10, ignoreMask);
                recoil = true;
                Invoke("StopRecoil", recoilTyme);
            }
        }
    }
}
