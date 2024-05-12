using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Database;
using Firebase.Extensions;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    FirebaseAuth auth;
    DatabaseReference dbRef;
    DatabaseReference dbUsersReference;

    public TMP_Text ProfileUsername, TicketNo;

    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        dbUsersReference = FirebaseDatabase.DefaultInstance.GetReference("Users");
    }

    void Start()
    {
        RetrieveUserData();
    }


    public FirebaseUser GetCurrentUser()
    {
        return auth.CurrentUser;
    }

    public void RetrieveUserData()
    {
        Firebase.Auth.FirebaseUser currentUser = auth.CurrentUser;
        if (currentUser != null)
        {
            string userId = currentUser.UserId;

            // Create a database reference for the user's data
            DatabaseReference userRef = dbRef.Child("Users").Child(userId);

            // Query playerQuery = dbRef.Child("Users").Child(userId);

            // Attach a listener to the database reference to read the data
            userRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve user data: " + task.Exception);
                    return;
                }

                // Parse the retrieved data
                DataSnapshot dataSnapshot = task.Result;
                if (dataSnapshot != null && dataSnapshot.Exists)
                {
                    // Deserialize the JSON data into a C# object
                    Users userData = JsonUtility.FromJson<Users>(dataSnapshot.GetRawJsonValue());

                    ProfileUsername.text = userData.username;
                    TicketNo.text = userData.tickets.ToString();

                }
                else
                {
                    Debug.LogWarning("User data does not exist.");
                }
            });
        }
    }

    public async Task<Users> GetUser(string uuid)
    {
        Query q = dbUsersReference.Child(uuid).LimitToFirst(1);
        Users users = null;

        await dbUsersReference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Sorry, there was an error retrieving player stats : ERROR " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot ds = task.Result;//path -> playerstats/$uuid
                if (ds.Child(uuid).Exists)
                {

                    //path to the datasapshot playerstats/$uuid/<we want this values>
                    users = JsonUtility.FromJson<Users>(ds.Child(uuid).GetRawJsonValue());

                    Debug.Log("ds... : " + ds.GetRawJsonValue());
                    Debug.Log("Users values.." + users.UsersToJson());

                }
            }
        });
        return users;
    }

    public void UpdateUserName(string uuid, string userName)
    {
        //update only specific properties that we want

        //path: leaderboards/&uuid/username
        //path: leaderboards/$uuid/totalMoney
        //path: leaderboards/$uuid/totalTmeSpent
        //path: leaderboards/$uuid/updatedOn
        Debug.Log("Where id: " + uuid);
        dbUsersReference.Child(uuid).Child("username").SetValueAsync(userName);
    }
}

   


    

