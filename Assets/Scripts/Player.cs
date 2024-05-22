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
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // gameEnded = false;
    }

    // Update is called once per frame
    private void Update()
    {
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
        Debug.Log("1ST TOUCH");
        Touch touch = Input.GetTouch(0);

        Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                deltaX = touchPos.x - transform.position.x;
                break;

            case TouchPhase.Moved:
                rb.MovePosition(new Vector2(touchPos.x - deltaX, transform.position.y));
                Debug.Log("MOVED");
                break;

            case TouchPhase.Ended:
                rb.velocity = Vector2.zero;
                break;
        }
    }
    private void HandleMouseInput()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            deltaX = mousePos.x - transform.position.x;
            Debug.Log("Mouse button down at: " + mousePos);
        }
        else if (Input.GetMouseButton(0))
        {
            rb.MovePosition(new Vector2(mousePos.x - deltaX, transform.position.y));
            Debug.Log("Mouse moved to: " + mousePos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            rb.velocity = Vector2.zero;
            Debug.Log("Mouse button up");
        }
    }
        // if (Input.GetMouseButton(0))
        // {
        //     Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //     if (touchPos.x < 0)
        //     {
        //         rb.AddForce(Vector2.left * moveSpeed);
        //     }
        //     else
        //     {
        //         rb.AddForce(Vector2.right * moveSpeed);
        //     }
        // }
        // else
        // {
        //     rb.velocity = Vector2.zero;
        // }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Rock")
        {
            gameEnded = true;
            gameMgr.SetGameEnded(gameEnded);
        }
    }
}
