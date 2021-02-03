using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ExplosionZone : MonoBehaviour
{
    [Range(1,100)] public int damage = 10;
    [Range(1, 100)] public float force = 10;
}
