using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrogGameManager : MonoBehaviour
{

    public GameObject rock;
    public float maxX;
    public Transform spawnPoint;
    public float spawnRate;

    bool gameStarted = false;
    public TMP_Text scoreText;
    int score = 0;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && !gameStarted)
        {
            StartSpawning();
            gameStarted = true;
        }
    }

    private void StartSpawning() 
    {
        InvokeRepeating("SpawnBlock", 0.5f, spawnRate);
        InvokeRepeating("AddScore", 0.5f, 1f);
    }

    void SpawnBlock()
    {
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.x = Random.Range(-maxX, maxX);
        Instantiate(rock, spawnPos, Quaternion.identity);
    }

    void AddScore() 
    {
        score++;
        scoreText.text = score.ToString();
    }
}