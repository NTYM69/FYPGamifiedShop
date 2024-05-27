using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watch : MonoBehaviour
{
    public float fallSpeed;
    public FrogGameManager gameMgr;

    // Update is called once per frame

    void Start()
    {
        gameMgr = FindObjectOfType<FrogGameManager>();
    }
    void Update()
    {
        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
        
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    public void UpdateFallSpeed(float newSpeed)
    {
        fallSpeed = newSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.name == "Player")
        {
            gameMgr.SlowObstacles();
            Destroy(gameObject);
        }
        
    }
}
