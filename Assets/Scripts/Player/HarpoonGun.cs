using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class HarpoonGun : MonoBehaviour
{
    [SerializeField, Range(0, 0.5f)]
    private float harpoonPullForce = 0.05f;
    [SerializeField, Range(0, 2)]
    private float harpoonDonePull = 1;
    [SerializeField, Range(0.1f, 5f)] 
    private float gunDirectionChangeSpeed = 1;
    [SerializeField] private GameObject harpoonModel;
    [SerializeField] private GameObject harpoon;
    [SerializeField] private LayerMask mask = -1;
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private LineRenderer lr;
    private GameObject lastHarpoon;
    private Harpoon harpoonComponent;
    void Update()
    {
        RaycastHit hit;
        float singleStep = gunDirectionChangeSpeed * Time.deltaTime;
        if (Physics.Raycast(transform.position, transform.forward, out hit, mask))
        {
            Vector3 lookDir = hit.point - harpoonModel.transform.position;
            lookDir = Vector3.RotateTowards(harpoonModel.transform.forward, lookDir, singleStep, 0.0f);
            harpoonModel.transform.rotation = Quaternion.LookRotation(lookDir);
        }
        else
        {
            Vector3 lookDir = Vector3.RotateTowards(harpoonModel.transform.forward, harpoonModel.transform.position + transform.forward * 1000, singleStep, 0.0f);
            harpoonModel.transform.rotation = Quaternion.LookRotation(lookDir);
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            LaunchGrappleHarpoon();
        }
        
        if (lastHarpoon)
        {
            lr.SetPosition(0, harpoonModel.transform.position);
            lr.SetPosition(1, lastHarpoon.transform.position);
            if (harpoonComponent.impact)
            {
                player.position = Vector3.Lerp(player.position, lastHarpoon.transform.position, harpoonPullForce);
                if (Vector3.Distance(player.position, lastHarpoon.transform.position) < harpoonDonePull)
                {
                    lastHarpoon = null;
                    harpoonComponent = null;
                    lr.enabled = false;
                }
            }
        }
    }

    private void LaunchGrappleHarpoon()
    {
        if (lastHarpoon != null)
        {
            Destroy(lastHarpoon);
        }
        Transform gun = harpoonModel.transform.GetChild(0);
        lastHarpoon = Instantiate(harpoon, gun);
        lastHarpoon.transform.parent = null;
        harpoonComponent = lastHarpoon.GetComponent<Harpoon>();
        lr.enabled = true;
    }
}
