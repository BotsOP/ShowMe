using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target : MonoBehaviour, IShootable
{
    public void GotHit()
    {
        Debug.Log("hit");
        Destroy(gameObject);
    }
}
