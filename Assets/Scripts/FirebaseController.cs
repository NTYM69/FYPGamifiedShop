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
    public TMP_Text LoginUsernameError, LoginPasswordError, RegUsernameError, RegPasswordError, RegCPasswordError, LoginChangePassNoti;
    private Dictionary<TMP_InputField, bool> inputFieldVisibility = new Dictionary<TMP_InputField, bool>(); // Map each showPasswordButton with its own visibility
    public FirebaseManager fbMgr;
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
        // Censor the password fields
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
        // Clear error texts
        LoginUsernameError.text = "";
        LoginPasswordError.text = "";

        // obtain email and password from fields
        string email = LoginUsernameField.text;
        string password = LoginPasswordField.text;

        // Show error if email is empty
        if (string.IsNullOrEmpty(email))
        {
            LoginUsernameError.text = "Please enter email";
            return;
        }

        // Show error if password is empty
        if (string.IsNullOrEmpty(password))
        {
            LoginPasswordError.text = "Please enter password";
            return;
        }

        // Sign the user in via Firebase Authentication
        SignInUser(email, password);
    } 

    public void RegisterUser()
    {   
        // Clear error texts
        RegUsernameError.text = "";
        RegPasswordError.text = "";
        RegCPasswordError.text = "";

        // obtain email and password from fields
        string email = RegUsernameField.text;
        string password = RegPasswordField.text;
        string confirmPassword = RegCPasswordField.text;

        // Show error if email is empty
        if (string.IsNullOrEmpty(email))
        {
            RegUsernameError.text = "Please enter email";
            return;
        }

        // Show error if password is empty
        if (string.IsNullOrEmpty(password))
        {
            RegPasswordError.text = "Please enter password";
            return;
        }

        // Show error if confirm password is empty
        if (string.IsNullOrEmpty(confirmPassword))
        {
            RegCPasswordError.text = "Please re-enter password";
            return;
        }

        // Show error if password does not match confirm password
        if (password != confirmPassword)
        {
            RegCPasswordError.text = "Password does not match";
            return;
        }

        // Create the user via Firebase Authentication
        CreateUser(email, password);
    }

    // CreateUser function provided by Firebase
    void CreateUser(string email, string password) 
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            // If create user has been cancelled
            if (task.IsCanceled) 
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled."); 
                return;
            }

            // If there is error in creating the user
            if (task.IsFaulted) 
            {
                FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException; 
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode; // Obtain the error code 

                switch(errorCode)
                {
                    case AuthError.InvalidEmail:
                        RegUsernameError.text = "Invalid email address format."; // Show error if email is invalid format
                        break;
                    
                    case AuthError.EmailAlreadyInUse:
                        RegUsernameError.text = "Email already in used. Try another one."; // Show error if the email is already in used
                        break;

                    case AuthError.WeakPassword:
                        RegPasswordError.text = "Password length must be at least 6."; // Show error if the password is invalid / weak
                        break;
                    
                    default:
                        RegUsernameError.text = "Registration failed"; // Default case if registation failed 
                        break;
                }
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
              
            // Creating the user and initializing the user's data
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            
            InitializeGameData(result.User.UserId);
            SceneManager.LoadScene(3); // Load user into the main menu after registration
               
        });
    }

    // SignInUser function provided by Firebase
    void SignInUser(string email, string password) 
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            // If sign in user has been cancelled
            if (task.IsCanceled) 
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }

            // If the user provided invalid login information
            if (task.IsFaulted) {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                LoginUsernameError.text = "Invalid login information";  // Display the error 
                return;
            }

            // Load the user into the main menu after successful sign in
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            SceneManager.LoadScene(3);
        });
    }

    // Toggle button function for password
    public void ShowPasswordButton(TMP_InputField inputField) 
    {
        if (inputFieldVisibility.ContainsKey(inputField))
        {
            // Toggle the current visibility state
            bool isHidden = inputFieldVisibility[inputField];
            inputField.contentType = isHidden ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password; // if true, content type stanrd, if false, content type password
            inputFieldVisibility[inputField] = !isHidden;
        }
        else
        {
            // If the input field is not in the dictionary, assume it is initially hidden
            inputField.contentType = TMP_InputField.ContentType.Standard; // Change to not hidden
            inputFieldVisibility[inputField] = false; // Map the inputField's visibility to false (not hidden)
        }

        inputField.ForceLabelUpdate(); // Update the field
    }
    
    // Create new Users class data for the user 
    void InitializeGameData(string userID) 
    {
        Users newUser = new Users("NewUser", 0, 0, 0, 0, DateTime.Now, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, new List<string>()); // Create new Users object
        string json = JsonUtility.ToJson(newUser); // Turn the data into json format
        dbRef.Child("Users").Child(userID).SetRawJsonValueAsync(json); // Update the database with the json format of the new user 
    }

    public async void ResetPassword()
    {
        string email = LoginUsernameField.text; // Obtain email from field

        // if email is not empty
        if (!string.IsNullOrEmpty(email))
        {
            await fbMgr.SendPasswordResetEmail(email); // Send password reset email to the user's email
            Debug.Log("Resetting password for email " + email);
            // Update the notification to alert the user
            LoginChangePassNoti.text = "A password reset link has been sent to your email address. Please check your inbox and follow the instructions to change your password.";
        }
    }
}
