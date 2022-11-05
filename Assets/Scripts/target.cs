using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target : MonoBehaviour, IShootable
{
    [SerializeField]
    private int health = 1;
    [SerializeField]
    private int amountPoints = 10;
    public void GotHit()
    {
        health--;
        if (health <= 0)
        {
            EventSystem<int>.RaiseEvent(EventType.SCORED_POINTS, amountPoints);
            Destroy(gameObject);
        }
    }
}
