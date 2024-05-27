using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{

    public float fallSpeed = 5f;
    // Update is called once per frame
    
    void Update()
    {
        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }

        // Move the rock downwards at the constant fall speed
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    public void UpdateFallSpeed(float newSpeed)
    {
        fallSpeed = newSpeed;
    }
}
