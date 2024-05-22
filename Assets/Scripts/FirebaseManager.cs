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

    private string email;

    private string uuid;

    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        dbUsersReference = FirebaseDatabase.DefaultInstance.GetReference("Users");
    }

    void Start()
    {
        uuid = GetCurrentUser().UserId;
    }


    public FirebaseUser GetCurrentUser()
    {
        return auth.CurrentUser;
    }

    public async Task SendPasswordResetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            Debug.LogError("No email provided for password reset.");
            return;
        }
        try
        {
            await auth.SendPasswordResetEmailAsync(email);
            Debug.Log("Password reset email sent successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + ex);
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
                DataSnapshot ds = task.Result;
                if (ds.Child(uuid).Exists)
                {
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
        dbUsersReference.Child(uuid).Child("username").SetValueAsync(userName);
    }

    public void UpdateLastLogin(string uuid) 
    {
        dbUsersReference.Child(uuid).Child("lastLogin").SetValueAsync(DateTime.Now.ToString("o"));
    }

    public void UpdateLastRedeemed(string uuid)
    {
        dbUsersReference.Child(uuid).Child("lastRedeemed").SetValueAsync(DateTime.Now.ToString("o"));
    }

    public async void AddDailyRedeemed(string uuid)
    {
        Users users = await GetUser(uuid);
        int userDailyRedeemed = users.dailyRedeemed;
        Debug.Log("user daily is :" + userDailyRedeemed);
        userDailyRedeemed++;
        Debug.Log("new user daily is :" + userDailyRedeemed);
        await dbUsersReference.Child(uuid).Child("dailyRedeemed").SetValueAsync(userDailyRedeemed);
    }

    public async Task<int> CheckDailyRedeemed(string uuid)
    {
         Users users = await GetUser(uuid);
         int userDailyRedeemed = users.dailyRedeemed;

         return userDailyRedeemed;
    }

    public void ResetDailyRedeemed(string uuid)
    {
        dbUsersReference.Child(uuid).Child("dailyRedeemed").SetValueAsync(0);
    }

    public async void addTickets(int amount)
    {
        Users users = await GetUser(uuid);
        int dailyAmount = users.dailyEarned;
        if (dailyAmount <= 100)
        {
            int amountToAdd = users.tickets + amount;
            // int dailyAmount = users.tickets + amount;
            await dbUsersReference.Child(uuid).Child("tickets").SetValueAsync(amountToAdd);
            await dbUsersReference.Child(uuid).Child("dailyEarned").SetValueAsync(amountToAdd);
        }
    }

    public void ResetDailyEarned(string uuid)
    {
        dbUsersReference.Child(uuid).Child("dailyEarned").SetValueAsync(0);
    }

    public void UpdateLastDailyReset(string uuid)
    {
        dbUsersReference.Child(uuid).Child("lastDailyReset").SetValueAsync(DateTime.Now.ToString("o"));
    }
}

   


    

