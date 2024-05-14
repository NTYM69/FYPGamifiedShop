using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
// using System.Threading.Tasks;
// // using Firebase;
// // using Firebase.Auth;
// using System;

public class ProfilePageManager : MonoBehaviour
{
    public FirebaseManager fbMgr;
    [SerializeField] private TMP_InputField editUserName;
    [SerializeField] private TMP_Text usernameTextNoti, changePassTextNoti;
    private string currentUserName;
    private string uuid;
    private float duration = 5f;
    private string email;
    
    //initialize uuid and email 
    void Start() 
    {
        uuid = fbMgr.GetCurrentUser().UserId;
        email = fbMgr.GetCurrentUser().Email;
        // Debug.Log("user's email : " + email);

        DisplayUserName(uuid);
    }

    // Method that will display user's username on input field's placeholder
    private async void DisplayUserName(string uuid)
    {
        Users users = await fbMgr.GetUser(uuid);
        currentUserName = users.username;
        editUserName.text = currentUserName;
        Debug.Log(currentUserName);
    }

    //Method to update the user's username
    public async void UpdateUserName()
    {

        Users users = await fbMgr.GetUser(uuid);
        currentUserName = editUserName.text;
        fbMgr.UpdateUserName(uuid, editUserName.text);
        StartCoroutine(ChangeTextRoutine());

    }

    IEnumerator ChangeTextRoutine() //Enumerator to display notification text for 5s
    {
        usernameTextNoti.text ="Name has been changed";
        yield return new WaitForSeconds(duration);
        usernameTextNoti.text = "";
    }

    public async void SendPasswordChangeEmail() //Method to send an email to user's email for password change
    {
         if (fbMgr.GetCurrentUser() != null) 
        {
            await fbMgr.SendPasswordResetEmail(email);
            StartCoroutine(ChangePasswordRoutine());
        }

    }

    IEnumerator ChangePasswordRoutine() { //Enumerator to display notification text for 5s
        changePassTextNoti.text = "A password reset link has been sent to your email address. Please check your inbox and follow the instructions to change your password.";
        yield return new WaitForSeconds(duration);
        changePassTextNoti.text = "";
    }

}

   

