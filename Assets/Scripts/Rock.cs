using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public float fallSpeed = 5f;
    
    void Update()
    {
        // If object passes the bottom of the screen, destroy it
        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }

        // Constantly move the object vertically downwards with fallspeed as times go on
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    // Update falling speed value of the object 
    public void UpdateFallSpeed(float newSpeed)
    {
        fallSpeed = newSpeed;
    }
}
