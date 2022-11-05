using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetMove2 : MonoBehaviour
{
    public AnimationCurve curve;
    public Transform target;
    public Transform target2;
    public float speed;
    
    void Update()
    {
        transform.position = Vector3.Lerp(target.position, target2.position, curve.Evaluate((Time.time / speed) % 1));
        Debug.Log($"{curve.Evaluate((Time.time / speed) % 1) }  {(Time.time / speed) % 1}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(target.position, target2.position);
    }
}
