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
        // if(Input.GetMouseButton(0) && !gameStarted)
        // {
        //     StartSpawning();
        //     gameStarted = true;
        // }

        if(gameStarted)
        {
            StartSpawning();
            tutorialCanvas.SetActive(false);
            gameStarted = false;
        
        }

        if (gameEnded)
        {
            HandleGameEnded();
            gameEnded = false;
        }
    }

    public void StartGame()
    {
        gameStarted = true;
    }

    private void StartSpawning() 
    {
        InvokeRepeating("SpawnBlock", 0.5f, spawnRate);
        InvokeRepeating("AddScore", 0.5f, 1f);
    }

    void SpawnBlock()
    {
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.x = UnityEngine.Random.Range(-maxX, maxX);
        Instantiate(rock, spawnPos, Quaternion.identity);
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

    public void HandleGameEnded()
    {
        Time.timeScale = 0f;
        endGameOverlay.SetActive(true);
        endScoreText.text = score.ToString();

        ticketWon = score / 10;

        int intTicketWon = (int)Math.Floor(ticketWon);

        ticketNo.text = "+" + intTicketWon.ToString();

        fbMgr.addTickets(intTicketWon);

        Debug.Log("Game has ended");

    }
}