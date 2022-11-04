using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target : MonoBehaviour, IShootable
{
    [SerializeField]
    private int health = 1;
    public void GotHit()
    {
        Debug.Log("hit");
        health--;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
