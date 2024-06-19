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
    public GameObject watch;
    public GameObject fortuneCat;
    public float maxX;
    public Transform spawnPoint;
    public float spawnRate;
    // public Button pauseButton, continueButton, endGameButton, finishGameButton;
    public GameObject pauseMenuCanvas, tutorialCanvas, endGameOverlay;
    bool gameStarted = false;
    public TMP_Text scoreText, ticketNo, endScoreText;
    int score = 0;
    public bool gameEnded = false;
    private float ticketWon;
    public FirebaseManager fbMgr;
    private float currentFallSpeed;

    void Start()
    {
        pauseMenuCanvas.SetActive(false);
        // Time.timeScale = 0f;
        tutorialCanvas.SetActive(true);
        endGameOverlay.SetActive(false);
        gameStarted = false;
    }
    // Update is called once per frame
    void Update()
    {

        if(gameStarted)
        {
            currentFallSpeed = 5f;
            rock.GetComponent<Rock>().UpdateFallSpeed(currentFallSpeed);
            StartSpawning();
            tutorialCanvas.SetActive(false);
            gameStarted = false;
        
        }

        if (gameEnded)
        {
            HandleGameEnded();
            gameEnded = false;
        }

        if (score == 20)
        {
            currentFallSpeed = 8f;
            rock.GetComponent<Rock>().UpdateFallSpeed(currentFallSpeed);
            CancelInvoke("SpawnBlock");
            InvokeRepeating("SpawnBlock", 1f, spawnRate);
        }

        if (score == 50)
        {
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
        InvokeRepeating("SpawnBlock", 1f, spawnRate);
        InvokeRepeating("AddScore", 0.1f, 1f);
        InvokeRepeating("SpawnPowerups", 7.5f, 30f);
    }

    void SpawnBlock()
    {
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.x = UnityEngine.Random.Range(-maxX, maxX);
        Instantiate(rock, spawnPos, Quaternion.identity);
    }
    
    void SpawnPowerups()
    {
        int RandomPower = UnityEngine.Random.Range(0, 2);
        Debug.Log("random : " + RandomPower);
        if (RandomPower == 0)
        {
            Vector3 spawnPos = spawnPoint.position;
            spawnPos.x = UnityEngine.Random.Range(-maxX, maxX);
            Instantiate(watch, spawnPos, Quaternion.identity);
        }
        else
        {
            Vector3 spawnPos = spawnPoint.position;
            spawnPos.x = UnityEngine.Random.Range(-maxX, maxX);
            Instantiate(fortuneCat, spawnPos, Quaternion.identity);
        }
        
    }

    void AddScore() 
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void PauseGame() 
    {
        Time.timeScale = 0f;
        pauseMenuCanvas.SetActive(true);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        pauseMenuCanvas.SetActive(false);
    }

    public void EndGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(3);
    }

    public void SetGameEnded(bool state)
    {
        gameEnded = state;
    }

    public float CalculateTickets(int score)
    {
        float maxTicket = 40f;
        float k = 0.025f;

        float tickets = maxTicket * (1 - Mathf.Exp(-k * score));
        
        return tickets; 
    }

    public async void HandleGameEnded()
    {
        Time.timeScale = 0f;
        endGameOverlay.SetActive(true);
        endScoreText.text = score.ToString();

        // ticketWon = score / 10;
        ticketWon = CalculateTickets(score);
        Debug.Log("initial ticket won: " + ticketWon);

        int intTicketWon = (int)Math.Floor(ticketWon);
        Debug.Log("Integer of ticket won: " + intTicketWon);

        ticketNo.text = "+" + intTicketWon.ToString();

        await fbMgr.AddTickets(intTicketWon);

        Debug.Log("Game has ended");

    }

    public void SlowObstacles()
    {
        // currentFallSpeed = currentFallSpeed / 2;
        // rock.GetComponent<Rock>().UpdateFallSpeed(currentFallSpeed);
        StartCoroutine(SlowTime());
    }

    public void DoublePoints()
    {
        StartCoroutine(DoubleScore());
    }

    IEnumerator SlowTime()
    {
        // currentFallSpeed = currentFallSpeed / 2;
        // rock.GetComponent<Rock>().UpdateFallSpeed(currentFallSpeed);
        // spawnRate *= 2;
        // CancelInvoke("SpawnBlock");
        // InvokeRepeating("SpawnBlock", 0.5f, spawnRate);
        
        // yield return new WaitForSeconds(10f); // Wait for 10 seconds

        // currentFallSpeed = currentFallSpeed * 2;
        // rock.GetComponent<Rock>().UpdateFallSpeed(currentFallSpeed);
        // spawnRate /= 2;
        // CancelInvoke("SpawnBlock");
        // InvokeRepeating("SpawnBlock", 0.5f, spawnRate);
        
        Time.timeScale = 0.5f;
        CancelInvoke("AddScore");
        InvokeRepeating("AddScore", 0.1f, 0.5f);
        yield return new WaitForSeconds(5f);
        Time.timeScale = 1f;
        CancelInvoke("AddScore");
        InvokeRepeating("AddScore", 0.1f, 1f);
    }

    IEnumerator DoubleScore()
    {
        CancelInvoke("AddScore");
        InvokeRepeating("AddScore", 0.1f, 0.5f);

        yield return new WaitForSeconds(10f);
        CancelInvoke("AddScore");
        InvokeRepeating("AddScore", 0.1f, 1f);

    }
}