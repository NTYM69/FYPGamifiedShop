using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private float deltaX;
    Rigidbody2D rb;
    public bool gameEnded = false;
    public FrogGameManager gameMgr;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Obtain rigidbody component
    }

    private void Update()
    {
        // Check if input is from touch screen or mouse
        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }     
        
        else if (Input.GetMouseButton(0))
        {
            HandleMouseInput();
        }
    }

    private void HandleTouchInput()
    {
        Touch touch = Input.GetTouch(0); // Obtain the first touch of the screen

        Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position); // Obtain position of where the user touches

        switch (touch.phase)
        {
            case TouchPhase.Began: // When user touches the screen
                deltaX = touchPos.x - transform.position.x; // Obtain difference between user touch position and the player position
                break;

            case TouchPhase.Moved: // When user moves while touching the screen
                rb.MovePosition(new Vector2(touchPos.x - deltaX, transform.position.y)); // Update new position of the player relative to the difference obtained above
                break;

            case TouchPhase.Ended: // When user stops touching
                rb.velocity = Vector2.zero; // Stops moving the player
                break;
        }
    }
    private void HandleMouseInput()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Obtain position of user's mouse cursor

        if (Input.GetMouseButtonDown(0)) // When the left mouse button is first pressed down
        {
            deltaX = mousePos.x - transform.position.x; // Obtain position of where the user mouse cursor is 
        }
        else if (Input.GetMouseButton(0)) // When the left mouse button is held down
        {
            rb.MovePosition(new Vector2(mousePos.x - deltaX, transform.position.y)); // Update new position of the player relative to the difference obtained above
        }
        else if (Input.GetMouseButtonUp(0)) // When the user lets go of their left mouse button
        {
            rb.velocity = Vector2.zero; // Stops moving the player
        }
    }

    // When player collides with the rock obstacle
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Rock")
        {
            gameEnded = true; 
            gameMgr.SetGameEnded(gameEnded); // Sets the gameEnded to true for the game manager
        }

    }
}
