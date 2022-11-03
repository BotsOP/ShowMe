using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class HarpoonGun : MonoBehaviour
{
    [SerializeField] private GameObject harpoon;
    [SerializeField] private float speed;
    [SerializeField] private LayerMask mask = -1;
    void Update()
    {
        RaycastHit hit;
        float singleStep = speed * Time.deltaTime;
        if (Physics.Raycast(transform.position, transform.forward, out hit, mask))
        {
            Vector3 lookDir = hit.point - harpoon.transform.position;
            lookDir = Vector3.RotateTowards(harpoon.transform.forward, lookDir, singleStep, 0.0f);
            harpoon.transform.rotation = Quaternion.LookRotation(lookDir);

            if (Input.GetMouseButtonDown(1))
            {
                
            }
        }
        else
        {
            Vector3 lookDir = Vector3.RotateTowards(harpoon.transform.forward, harpoon.transform.position + transform.forward * 1000, singleStep, 0.0f);
            harpoon.transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    private void LaunchGrappleHarpoon()
    {
        
    }
}
