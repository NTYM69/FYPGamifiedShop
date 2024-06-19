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

    public async Task UpdateUserName(string uuid, string userName)
    {
        await dbUsersReference.Child(uuid).Child("username").SetValueAsync(userName);
    }

    public async Task UpdateLastLogin(string uuid) 
    {
        await dbUsersReference.Child(uuid).Child("lastLogin").SetValueAsync(DateTime.Now.ToString("o"));
    }

    public async Task UpdateLastRedeemed(string uuid)
    {
        await dbUsersReference.Child(uuid).Child("lastRedeemed").SetValueAsync(DateTime.Now.ToString("o"));
    }

    public async Task AddDailyRedeemed(string uuid)
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

    public async Task ResetDailyRedeemed(string uuid)
    {
        await dbUsersReference.Child(uuid).Child("dailyRedeemed").SetValueAsync(0);
    }

    public async Task AddTickets(int amount)
    {
        Users users = await GetUser(uuid);
        int dailyAmount = users.dailyEarned;
        Debug.Log("Amount retrieved: " + users.tickets);
        if (dailyAmount <= 100)
        {
            int amountToAdd = users.tickets + amount;
            int updateDailyEarned = dailyAmount + amount;
            // int dailyAmount = users.tickets + amount;
            await dbUsersReference.Child(uuid).Child("tickets").SetValueAsync(amountToAdd);
            await dbUsersReference.Child(uuid).Child("dailyEarned").SetValueAsync(updateDailyEarned);
        }
    }

    public async Task ResetDailyEarned(string uuid)
    {
        await dbUsersReference.Child(uuid).Child("dailyEarned").SetValueAsync(0);
    }

    public async Task UpdateLastDailyReset(string uuid)
    {
        await dbUsersReference.Child(uuid).Child("lastDailyReset").SetValueAsync(DateTime.Now.ToString("o"));
    }

    public async Task PurchaseItem(string id, int cost)
    {
        Users users = await GetUser(uuid);
        if(id == "spinthewheel")
        {
            int spinTicketsToAdd = users.spinTickets + 1;
            await dbUsersReference.Child(uuid).Child("spinTickets").SetValueAsync(spinTicketsToAdd);
        }
        else
        {
            List<string> vouchersRetrieved = new List<string>();
            vouchersRetrieved = users.vouchers;

            foreach(var vouchers in vouchersRetrieved)
            {
                Debug.Log("Current vouchers: " + vouchers);
            }

            vouchersRetrieved.Add(id);
            await dbUsersReference.Child(uuid).Child("vouchers").SetValueAsync(vouchersRetrieved);

            foreach(var vouchers in vouchersRetrieved)
            {
                Debug.Log("Added vouchers: " + vouchers);
            }
        }

        if (users.tickets >= cost)
        {
            int amountToDeduct = users.tickets - cost;
            await dbUsersReference.Child(uuid).Child("tickets").SetValueAsync(amountToDeduct); 
        }

    }
    public async Task UseSpinTicket()
    {
        Users users = await GetUser(uuid);
        int updatedSpinTickets = users.spinTickets - 1;
        await dbUsersReference.Child(uuid).Child("spinTickets").SetValueAsync(updatedSpinTickets);
    }

    public async Task AddTicketsWithoutDaily(int amount)
    {
        Users users = await GetUser(uuid);
        Debug.Log("Amount retrieved: " + users.tickets);
        int amountToAdd = users.tickets + amount;
        await dbUsersReference.Child(uuid).Child("tickets").SetValueAsync(amountToAdd);
    }
}

   


    

