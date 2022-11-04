using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool impact;
    [SerializeField, Range(0, 5)]
    private float speed = 1;
    [SerializeField, Range(5, 60f)]
    private float timeUntilDestroyed = 10;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private bool isHarpoonBullet;

    private Rigidbody rb;

    private void Awake()
    {
        Destroy(gameObject, timeUntilDestroyed);
    }
    private void FixedUpdate()
    {
        Vector3 dir = isHarpoonBullet ? transform.up : transform.forward;
        transform.position += dir * speed;
    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        if ((mask & (1 << collisionInfo.gameObject.layer)) != 0)
        {
            collisionInfo.gameObject.GetComponent<IShootable>().GotHit();
        }
        
        Destroy(gameObject);
    }
}
