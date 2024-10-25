using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plasticBag : MonoBehaviour
{
    public float fallSpeed;
    public FrogGameManager gameMgr;

    void Start()
    {
        // Finds the game manager
        gameMgr = FindObjectOfType<FrogGameManager>();
    }
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

    // If player and object collides
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.name == "Player")
        {
            gameMgr.SlowObstacles(); // Slows down powerup
            Destroy(gameObject); // Remove object after collecting powerup
        }
        
    }
}
