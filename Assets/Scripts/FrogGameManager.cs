using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;


public class FrogGameManager : MonoBehaviour
{
    public GameObject rock;
    public GameObject plasticBag;
    public GameObject crushedCan;
    public float maxX;
    public Transform spawnPoint;
    public float spawnRate;
    public GameObject pauseMenuCanvas, tutorialCanvas, endGameOverlay, pauseButton;
    bool gameStarted = false;
    public TMP_Text scoreText, ticketNo, endScoreText;
    int score = 0;
    public bool gameEnded = false;
    private float ticketWon;
    public FirebaseManager fbMgr;
    private float currentFallSpeed;
    public GameObject bagToolTip, canToolTip, toolTipCancelButton;

    void Start()
    {
        // Set required canvases state to true or false
        pauseMenuCanvas.SetActive(false);
        tutorialCanvas.SetActive(true);
        endGameOverlay.SetActive(false);
        gameStarted = false; // Initialise game haven't start yet
    }

    void Update()
    {

        if(gameStarted)
        {
            currentFallSpeed = 5f; // Initial fall speed of object
            rock.GetComponent<Rock>().UpdateFallSpeed(currentFallSpeed); // Update the current rock prefab's speed
            StartSpawning(); // Start spawning the obstacles
            tutorialCanvas.SetActive(false); // Closes tutorial canvas
            gameStarted = false; // Set to false so that this portion is only called once
        
        }

        if (gameEnded)
        {
            HandleGameEnded();
            gameEnded = false; // Set to false so that this portion only called once
        }

        if (score == 20) 
        {
            // Increase the speed of the rock prefab when the score is 20
            currentFallSpeed = 8f; 
            rock.GetComponent<Rock>().UpdateFallSpeed(currentFallSpeed);
            CancelInvoke("SpawnBlock");
            InvokeRepeating("SpawnBlock", 1f, spawnRate);
        }

        if (score == 50)
        {
            // Increase the speed of the rock prefab when the score is 50
            currentFallSpeed = 15f;
            rock.GetComponent<Rock>().UpdateFallSpeed(currentFallSpeed);
            spawnRate = 0.5f;
            CancelInvoke("SpawnBlock");
            InvokeRepeating("SpawnBlock", 1f, spawnRate);
        }
    }

    public void StartGame()
    {
        gameStarted = true;
    }

    private void StartSpawning() 
    {
        InvokeRepeating("SpawnBlock", 1f, spawnRate); // Start spawning the rocks
        InvokeRepeating("AddScore", 0.1f, 1f); // Start counting the score
        InvokeRepeating("SpawnPowerups", 7.5f, 30f); // Start spawning the powerups 
    }

    void SpawnBlock()
    {
        // The spawn point / spawn area of the rocks
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.x = UnityEngine.Random.Range(-maxX, maxX);
        Instantiate(rock, spawnPos, Quaternion.identity);
    }
    
    void SpawnPowerups()
    {
        // Randomly choose between 2 powerups to be spawned
        int RandomPower = UnityEngine.Random.Range(0, 2);
        
        if (RandomPower == 0)
        {
            // Spawns plastic bag powerup
            Vector3 spawnPos = spawnPoint.position;
            spawnPos.x = UnityEngine.Random.Range(-maxX, maxX);
            Instantiate(plasticBag, spawnPos, Quaternion.identity);
        }
        else
        {
            // Spawns crushed can powerup
            Vector3 spawnPos = spawnPoint.position;
            spawnPos.x = UnityEngine.Random.Range(-maxX, maxX);
            Instantiate(crushedCan, spawnPos, Quaternion.identity);
        }
    }

    void AddScore() // Increments the score by 1 and updates the score text
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void PauseGame() 
    {
        // Pauses the game while showing the pause menu
        Time.timeScale = 0f; 
        pauseMenuCanvas.SetActive(true);
    }

    public void ContinueGame()
    {
        // Continues the game while closing the pause menu
        Time.timeScale = 1f;
        pauseMenuCanvas.SetActive(false);
    }

    public void EndGame()
    {
        // Resume speed from pause and quits back to the main menu
        Time.timeScale = 1f;
        SceneManager.LoadScene(3);
    }

    public void SetGameEnded(bool state)
    {
        gameEnded = state;
    }

    public float CalculateTickets(int score)
    {
        // Set max ticket a user can get
        float maxTicket = 40f;
        float k = 0.025f;

        float tickets = maxTicket * (1 - Mathf.Exp(-k * score)); // Formula to calculate the ticket earned by user based on their score in a diminishing return way
        
        return tickets; 
    }

    public async void HandleGameEnded()
    {
        // Stops game and opens end game overlay
        Time.timeScale = 0f;
        endGameOverlay.SetActive(true);
        pauseButton.SetActive(false);
        endScoreText.text = score.ToString();

        ticketWon = CalculateTickets(score); // Use formula to calculate ticket based on score
        int intTicketWon = (int)Math.Floor(ticketWon); // Round down the calculated ticket from the formula
        ticketNo.text = "+" + intTicketWon.ToString(); // Display ticket won amount

        await fbMgr.AddTickets(intTicketWon); // Adds ticket to database for the user
    }

    public void SlowObstacles()
    {
        // Powerup that slows down the obstacle
        StartCoroutine(SlowTime());
    }

    public void DoublePoints()
    {
        // Powerup that doubles the point
        StartCoroutine(DoubleScore());
    }

    IEnumerator SlowTime()
    {  
        // Half the game speed but double the score to compensate
        Time.timeScale = 0.5f;
        CancelInvoke("AddScore");
        InvokeRepeating("AddScore", 0.1f, 0.5f);
        
        // Since game speed is halved, slow down lasts for 10 seconds
        yield return new WaitForSeconds(5f);

        // Resume the game speed and add score to normal
        Time.timeScale = 1f;
        CancelInvoke("AddScore");
        InvokeRepeating("AddScore", 0.1f, 1f);
    }

    IEnumerator DoubleScore()
    {
        // Double frequency of AddScore
        CancelInvoke("AddScore");
        InvokeRepeating("AddScore", 0.1f, 0.5f);
        
        // Powerup lasts for 10 seconds
        yield return new WaitForSeconds(10f);
        
        // Resume the normal AddScore
        CancelInvoke("AddScore");
        InvokeRepeating("AddScore", 0.1f, 1f);

    }

    public void OpenBagTooltip()
    {
        // Opens tooltip for plastic bag powerup
        bagToolTip.SetActive(true);
        toolTipCancelButton.SetActive(true);
    }
    public void OpenCanTooltip()
    {
        // Opens tooltip for crushed can powerup
        canToolTip.SetActive(true);
        toolTipCancelButton.SetActive(true);
    }
    public void CloseTooltip()
    {
        // Closes tootip
        bagToolTip.SetActive(false);
        canToolTip.SetActive(false);
        toolTipCancelButton.SetActive(false);
    }
}