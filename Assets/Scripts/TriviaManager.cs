using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class TriviaManager : MonoBehaviour
{
    [System.Serializable]
    public class Question {
        public int correctAnswerIndex;
    }
    public Question[] questions;
    public Button[] buttons;
    public Button nikeTriviaButton;
    private int currentQuestionIndex = 0;
    private int correctAnswer;
    private int score = 0;
    public GameObject triviaListCanvas, finalResultCanvas, rewardText, nikeCompletedText;
    public Image rewardImage;
    public TMP_Text scoreText;
    public GameObject[] NikeQuestionCanvas;
    public FirebaseManager fbMgr;
    string uuid;
    void Start()
    {
        uuid = fbMgr.GetCurrentUser().UserId;
        triviaListCanvas.SetActive(true);
        // Disable the questions and other canvases first
        foreach (var questionCanvas in NikeQuestionCanvas)
        {
            questionCanvas.SetActive(false);
        }
        finalResultCanvas.SetActive(false);
        rewardText.SetActive(false);
        CheckNikeTriviaAvailability();
    }

    public void OpenNikeTrivia()
    {
        // When user starts the trivia
        triviaListCanvas.SetActive(false);
        NikeQuestionCanvas[currentQuestionIndex].SetActive(true); // starts with 0 so first question is displayed
        score = 0;
    }
    
    // When user answers a question
    public void ButtonPressed (int btnNo)
    {
        // Assign the correct answer to an index
        correctAnswer = questions[currentQuestionIndex].correctAnswerIndex;
        for (int i = 0; i<3; i++)
        {
            // Every question has 3 buttons, but they are in the same list, so it stacks up to n * 3, where n = number of questions and count starts from 0
            buttons[i + (currentQuestionIndex * 3)].interactable = false; // disable the buttons after user answered
        }
        if (btnNo == correctAnswer)
        {
            // Increment score if user got the question correct
            score++;
            buttons[btnNo + (currentQuestionIndex * 3)].GetComponent<Image>().color = Color.green; // Apply green background to button for the correct answer
            if (currentQuestionIndex < questions.Length - 1)
            {
                // If its not the last question, go to next question after 2 seconds
                Invoke("NextQuestion", 2);
            }
            else
            {   // display final results after 2 seconds
                Invoke("FinalResults", 2);
            }
        }
        else
        {
            // When the user answers inccorrectly
            buttons[(currentQuestionIndex * 3) + correctAnswer].GetComponent<Image>().color = Color.green; // Apply green background to button for the correct answer
            buttons[btnNo + (currentQuestionIndex * 3)].GetComponent<Image>().color = Color.red; // Apply red background to the incorrect answer 
            if (currentQuestionIndex < questions.Length - 1)
            {
                // If its not the last question, go to next question after 2 seconds
                Invoke("NextQuestion", 2);
            }
            else
            {
                // display final results after 2 seconds
                Invoke("FinalResults", 2);
            }
        }
    }

    public void NextQuestion()
    {
        // Disable current question canves and enable the next one
        NikeQuestionCanvas[currentQuestionIndex].SetActive(false);
        currentQuestionIndex++;
        NikeQuestionCanvas[currentQuestionIndex].SetActive(true);
    }

    public async void FinalResults()
    {
        // Enable final result popup and display score
        finalResultCanvas.SetActive(true);
        scoreText.text = score.ToString() + "/5";
        if (score == 5)
        {
            // When user got all the questions right, reward with voucher and disable the nike trivia button
            nikeTriviaButton.interactable=false;
            nikeCompletedText.SetActive(true);
            Sprite itemSprite = Resources.Load<Sprite>("ShopItemImages/nike5");
            if (itemSprite != null)
            {
                rewardImage.sprite = itemSprite;
            }
            else
            {
                Debug.Log("Error, image not found for nike5.");
            }
            await fbMgr.PurchaseItem("nike5", 0); // Update the user vouchers list
            await fbMgr.UpdateLastNikeTrivia(); // Update the last nike trivia time for the user
        }
        else
        {
            // Enable no reward text
            rewardText.SetActive(true);
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(3); 
    }

    public async void CheckNikeTriviaAvailability()
    {
        Users users = await fbMgr.GetUser(uuid);
        DateTime lastNikeTriviaDate; 
        DateTime currentDate = DateTime.UtcNow.Date; // Obtain current date
        DateTime threeDaysAdded;

        // Assume that user has never played the trivia before if date is empty or null
        if (string.IsNullOrEmpty(users.lastNikeTrivia)) 
        {
            nikeTriviaButton.interactable=true; // Allow user to play 
            return;
        }
        else
        {
            lastNikeTriviaDate = DateTime.Parse(users.lastNikeTrivia).Date; // Obtain user's last nike trivia date
            threeDaysAdded = lastNikeTriviaDate.AddDays(3); // Add 3 days to that date
        }

        Debug.Log("three days : "+ threeDaysAdded);
        Debug.Log("current date : " + currentDate);

        if (threeDaysAdded > currentDate) // When 3 days have yet to passed from the last nike trivia date
        {
            // Disable the trivia
            nikeTriviaButton.interactable=false;
            nikeCompletedText.SetActive(true);
        }
        else
        {
            // Enable the trivia
            nikeTriviaButton.interactable=true;
        }
    }
}
