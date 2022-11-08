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
    private float gunDirectionChangeSpeed = 5;
    [SerializeField] private Transform harpoonGunPos;
    [SerializeField] private GameObject harpoonGrappleHook;
    [SerializeField] private GameObject harpoonBullet;
    [SerializeField] private GameObject bullet;
    [SerializeField] private LayerMask mask = -1;
    [SerializeField] private Transform player;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private float knockbackVelocity = 10f;
    private Rigidbody rb;
    private GameObject lastHarpoon;
    private Harpoon harpoonComponent;
    private bool inSuit;
    
    
    private void OnEnable()
    {
        EventSystem<bool>.Subscribe(EventType.CHANGED_SUIT, ChangeWaterSuit);
    }
    private void OnDestroy()
    {
        EventSystem<bool>.Unsubscribe(EventType.CHANGED_SUIT, ChangeWaterSuit);
    }

    private void ChangeWaterSuit(bool suit)
    {
        inSuit = suit;
    }

    private void Awake()
    {
        rb = player.gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        RotateGunToPointing();
        if (inSuit)
        {
            GrappleHarpoon();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Transform gun = harpoonGunPos.GetChild(0);
            if (!inSuit)
            {
                rb.AddForce(knockbackVelocity * gun.forward.normalized, ForceMode.VelocityChange);
            }
            Instantiate(inSuit ? harpoonBullet : bullet, harpoonGunPos).transform.parent = null;
        }
    }
    private void RotateGunToPointing()
    {
        RaycastHit hit;
        float singleStep = gunDirectionChangeSpeed * Time.deltaTime;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, mask))
        {
            Vector3 lookDir = hit.point - harpoonGunPos.position;
            lookDir = Vector3.RotateTowards(harpoonGunPos.forward, lookDir, singleStep, 0.0f);
            harpoonGunPos.rotation = Quaternion.LookRotation(lookDir);
            
            Debug.DrawLine(transform.position, hit.point, Color.blue);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
            Vector3 lookDir = Vector3.RotateTowards(harpoonGunPos.forward, harpoonGunPos.position + transform.forward * 1000 - harpoonGunPos.position, singleStep, 0.0f);
            harpoonGunPos.rotation = Quaternion.LookRotation(lookDir);
        }
    }
    private void GrappleHarpoon()
    {

        if (Input.GetMouseButtonDown(1))
        {
            LaunchGrappleHarpoon();
        }

        if (lastHarpoon)
        {
            lr.SetPosition(0, harpoonGunPos.position);
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
        Transform gun = harpoonGunPos.GetChild(0);
        lastHarpoon = Instantiate(harpoonGrappleHook, gun);
        lastHarpoon.transform.parent = null;
        harpoonComponent = lastHarpoon.GetComponent<Harpoon>();
        lr.enabled = true;
    }
}
