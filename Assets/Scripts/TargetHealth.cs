using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHealth : MonoBehaviour
{
    public float health = 5f;
    public GameObject Target;

    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(Target);
        }
    }
}
