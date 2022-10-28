using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    Element currentElement;
    public float damageToHealth = 10f;
    public float damageToShield;
    public float range = 600f;
    public float fireRate;   //measured in RPM 
    public float bulletSpeed;
    public int MaxMagSize;
    public int currentMagSize;
    public float knockBackForce;


    private float nextTimeToFire = 0f;
    private float reloadTimer = 0f;

    [Header("References")]
    //PlayerMovement player;
    //ShotBehavior shotBehavior;
    public GameObject bulletPrefab;
    public Camera fpsCam;
    public Transform bulletPos;
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        currentMagSize = MaxMagSize;
        damageToShield = damageToHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentMagSize > 0)
        {
            nextTimeToFire = Time.time + 60f/fireRate;
            Shoot();
            KnockBack(fpsCam.transform.forward, - knockBackForce);
        }
        if (currentMagSize <=0 || Input.GetKey(KeyCode.R) && currentMagSize < MaxMagSize)
        {
            ReloadWeapon(1, MaxMagSize - currentMagSize);
        }

        SwapElement();  // temp code for element swapping testing
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            //currentMagSize--;

            //Debug.Log(hit.transform.name);
            //Target target = hit.transform.GetComponent<Target>();
            //if (target != null)
            //{
            //    switch (currentElement)
            //    {
            //        case Element.fire:
            //            // write here what happens when the element is fire
            //            Debug.Log("Fire element is now active");
            //            //damageToHealth = 15;
            //            break;
            //        case Element.ice:
            //            // write here what happens when the element is ice
            //            Debug.Log("Ice element is now active");
            //            break;
            //        case Element.shock:
            //            // write here what happens when the element is shock
            //            Debug.Log("Shock element is now active");
            //            break;
            //        default:
            //            currentElement = Element.none;
            //            Debug.Log("NO element is now active");
            //            target.TakeDamage(damageToHealth); //change to check if shield or health
            //            break;
            //    }
            //}

            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletPos.transform.position, bulletPos.transform.rotation) as GameObject;
            //bullet.GetComponent<ShotBehavior>().setTarget(hit.point);
            bullet.GetComponent<Rigidbody>().velocity = bulletPos.forward * bulletSpeed; 
        }
    }

    void ReloadWeapon(float time, int amount)
    {
        reloadTimer += Time.deltaTime;
        if (reloadTimer >= time)
        {
            currentMagSize += amount;
            reloadTimer = 0;
        }
    }
    void SwapElement()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentElement = Element.fire;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentElement = Element.ice;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentElement = Element.ice;
        }
    }

    void KnockBack(Vector3 direction, float force)
    {
        //player.rb.AddForce(force * direction, ForceMode.Impulse);
    }
}
public enum Element
{
    none, fire, ice, shock
}
