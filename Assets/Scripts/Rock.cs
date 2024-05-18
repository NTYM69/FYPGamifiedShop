using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
        
    }
}
