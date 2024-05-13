using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Database;
using System;

public class FirebaseController : MonoBehaviour
{
    public TMP_InputField LoginUsernameField, LoginPasswordField, RegUsernameField, RegPasswordField, RegCPasswordField;
    public TMP_Text LoginUsernameError, LoginPasswordError, RegUsernameError, RegPasswordError, RegCPasswordError;

    
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    DatabaseReference dbRef;
 

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

     public void LoginUser()
    {
        LoginUsernameError.text = "";
        LoginPasswordError.text = "";

        string email = LoginUsernameField.text;
        string password = LoginPasswordField.text;

        if (string.IsNullOrEmpty(email))
        {
            LoginUsernameError.text = "Please enter email";
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            LoginPasswordError.text = "Please enter password";
            return;
        }

        SignInUser(email, password);
    } 

    public void RegisterUser()
    {
        RegUsernameError.text = "";
        RegPasswordError.text = "";
        RegCPasswordError.text = "";

        string email = RegUsernameField.text;
        string password = RegPasswordField.text;
        string confirmPassword = RegCPasswordField.text;

        if (string.IsNullOrEmpty(email))
        {
            RegUsernameError.text = "Please enter email";
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            RegPasswordError.text = "Please enter password";
            return;
        }

        if (string.IsNullOrEmpty(confirmPassword))
        {
            RegCPasswordError.text = "Please re-enter password";
            return;
        }

        if (password != confirmPassword)
        {
            RegCPasswordError.text = "Password does not match";
            return;
        }

        CreateUser(email, password);
    }

    void CreateUser(string email, string password) {
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
                if (task.IsCanceled) {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
            }
              
                // Firebase user has been created.
                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
                
                InitializeGameData(result.User.UserId);

                SceneManager.LoadScene(3);
               
    });
    }

    public void SignInUser(string email, string password) {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled) {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                LoginUsernameError.text = "Invalid login information";  

                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            SceneManager.LoadScene(3);
        });
    }

//     void InitializeFirebase() {
//   auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
//   auth.StateChanged += AuthStateChanged;
//   AuthStateChanged(this, null);
// }

// void AuthStateChanged(object sender, System.EventArgs eventArgs) {
//   if (auth.CurrentUser != user) {
//     bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
//         && auth.CurrentUser.IsValid();
//     if (!signedIn && user != null) {
//       Debug.Log("Signed out " + user.UserId);
//     }
//     user = auth.CurrentUser;
//     if (signedIn) {
//       Debug.Log("Signed in " + user.UserId);
//     //   displayName = user.DisplayName ?? "";
//     //   emailAddress = user.Email ?? "";
//     //   photoUrl = user.PhotoUrl ?? "";
//     }
//   }
// }

// void OnDestroy() {
//   auth.StateChanged -= AuthStateChanged;
//   auth = null;
// }
    
void InitializeGameData(string userID) {
     Users newUser = new Users("NewUser", 0, 0, 0, 0, DateTime.Now, new List<string>());
     string json = JsonUtility.ToJson(newUser);

     dbRef.Child("Users").Child(userID).SetRawJsonValueAsync(json);
}

}
