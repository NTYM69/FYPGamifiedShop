using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// using Firebase.Database;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    
    public TMP_Text ProfileUsername, TicketNo;
    // DatabaseReference dbRef;
    Firebase.Auth.FirebaseAuth auth;

    void Start() {
        auth = FirebaseAuth.DefaultInstance;
        // dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        
        // RetrieveUserData();
    }

    // public void RetrieveUserData() {
    //     Firebase.Auth.FirebaseUser currentUser = auth.CurrentUser;
    //     if (currentUser != null) {
    //         string userId = currentUser.UserId;

    //         // Create a database reference for the user's data
    //         DatabaseReference userRef = dbRef.Child("Users").Child(userId);

    //         // Query playerQuery = dbRef.Child("Users").Child(userId);

    //         // Attach a listener to the database reference to read the data
    //         userRef.GetValueAsync().ContinueWith(task =>
    //         {
    //             if (task.IsFaulted)
    //             {
    //                 Debug.LogError("Failed to retrieve user data: " + task.Exception);
    //                 return;
    //             }

    //             // Parse the retrieved data
    //             DataSnapshot dataSnapshot = task.Result;
    //             if (dataSnapshot != null && dataSnapshot.Exists)
    //             {
    //                 // Deserialize the JSON data into a C# object
    //                 GameData userData = JsonUtility.FromJson<GameData>(dataSnapshot.GetRawJsonValue());

    //                 ProfileUsername.text = userData.username;
    //                 TicketNo.text = userData.tickets.ToString();

    //             }
    //             else
    //             {
    //                 Debug.LogWarning("User data does not exist.");
    //             }
    //         });
    //     }
    // }
}

   


    

