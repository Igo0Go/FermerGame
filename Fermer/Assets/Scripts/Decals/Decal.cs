using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decal : MonoBehaviour
{
    public void Init(float time)
    {
        Destroy(gameObject, time);
    }
}
