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

    private Dictionary<TMP_InputField, bool> inputFieldVisibility = new Dictionary<TMP_InputField, bool>();

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    DatabaseReference dbRef;
 
    void Awake() 
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }
    void Start()
    {
        if (LoginPasswordField != null)
        {
            LoginPasswordField.contentType = TMPro.TMP_InputField.ContentType.Password;
        }
        if (RegPasswordField != null)
        {
            RegPasswordField.contentType = TMPro.TMP_InputField.ContentType.Password;
        }
        if (RegCPasswordField != null)
        {
            RegCPasswordField.contentType = TMPro.TMP_InputField.ContentType.Password;
        }
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

     public void ShowPasswordButton(TMP_InputField inputField) 
    {
        if (inputFieldVisibility.ContainsKey(inputField))
        {
            // Toggle the current visibility state
            bool isHidden = inputFieldVisibility[inputField];
            inputField.contentType = isHidden ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
            inputFieldVisibility[inputField] = !isHidden;
        }
        else
        {
            // If the input field is not in the dictionary, assume it is initially hidden
            inputField.contentType = TMP_InputField.ContentType.Standard;
            inputFieldVisibility[inputField] = false;
        }

        inputField.ForceLabelUpdate();
    }
    
void InitializeGameData(string userID) {
    Users newUser = new Users("NewUser", 0, 0, 0, 0, DateTime.Now, DateTime.MinValue, DateTime.MinValue, new List<string>());
    string json = JsonUtility.ToJson(newUser);
    newUser.lastRedeemed = DateTime.MinValue.ToString("o");
    newUser.lastDailyReset = DateTime.MinValue.ToString("o");
    dbRef.Child("Users").Child(userID).SetRawJsonValueAsync(json);
}

}
