using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    public bool impact;
    [SerializeField, Range(0, 2)]
    private float speed = 1;
    [SerializeField, Range(0, 0.1f)]
    private float delayImpact = 0;
    [SerializeField, Range(5, 60f)]
    private float timeUntilDestroyed = 10;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, timeUntilDestroyed);
    }
    private void FixedUpdate()
    {
        if (!impact)
        {
            transform.position += transform.up * speed;
        }
    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        StartCoroutine(ImpactDelay());
    }

    IEnumerator ImpactDelay()
    {
        yield return new WaitForSeconds(delayImpact);
        impact = true;
    }
}
