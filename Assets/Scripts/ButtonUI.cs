using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;

public class ButtonUI : MonoBehaviour
{
    public void LoginButton() {
        SceneManager.LoadScene(1);
    }

    public void RegisterButton() {
        SceneManager.LoadScene(2);
    }

    public void backLandingButton() {
        SceneManager.LoadScene(0);
    }

    public void backMainMenu() {
        SceneManager.LoadScene(3);
    }

    public void goProfilePage() {
        SceneManager.LoadScene(4);
    }

    public void goShopPage() {
        SceneManager.LoadScene(5);
    }

    public void signOutButton() {

        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        // Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        auth.SignOut();

        SceneManager.LoadScene(0);
    }
}
