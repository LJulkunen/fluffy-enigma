using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowKitsulope : MonoBehaviour
{

    public GameObject kitsulope;

    void Update()
    {
        transform.position = kitsulope.transform.position + Vector3.up * 0.63f;
    }
}
