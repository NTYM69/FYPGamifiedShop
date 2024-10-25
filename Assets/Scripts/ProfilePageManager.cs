using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfilePageManager : MonoBehaviour
{
    public FirebaseManager fbMgr;
    [SerializeField] private TMP_InputField editUserName;
    [SerializeField] private TMP_Text usernameTextNoti, changePassTextNoti;
    private string uuid;
    private float duration = 5f;
    private string email;
    
    void Start() 
    {
        // Obtain uuid and email
        uuid = fbMgr.GetCurrentUser().UserId;
        email = fbMgr.GetCurrentUser().Email;

        DisplayUserName();
    }

    // Display user's username in the input field
    private async void DisplayUserName()
    {
        Users users = await fbMgr.GetUser(uuid);
        editUserName.text = users.username;
    }

    // Update the user's username
    public async void UpdateUserName()
    {
        Users users = await fbMgr.GetUser(uuid);
        await fbMgr.UpdateUserName(editUserName.text); // Updates username in database
        StartCoroutine(ChangeTextRoutine()); 

    }

    IEnumerator ChangeTextRoutine() //Enumerator to display notification text for 5s
    {
        usernameTextNoti.text ="Name has been changed";
        yield return new WaitForSeconds(duration);
        usernameTextNoti.text = "";
    }

    public async void SendPasswordChangeEmail() // Send an email to user's email for password change
    {
         if (fbMgr.GetCurrentUser() != null) 
        {
            await fbMgr.SendPasswordResetEmail(email); // Send password reset email provided by Firebase
            StartCoroutine(ChangePasswordRoutine());
        }
    }

    IEnumerator ChangePasswordRoutine() { //Enumerator to display notification text for 5s
        changePassTextNoti.text = "A password reset link has been sent to your email address. Please check your inbox and follow the instructions to change your password.";
        yield return new WaitForSeconds(duration);
        changePassTextNoti.text = "";
    }

    public void signOutButton() 
    {
        // Signs out user and navigate them to landing page
        fbMgr.SignOutUser();
        SceneManager.LoadScene(0);
    }

}

   

