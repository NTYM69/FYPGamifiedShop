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

    private string uuid;

    void Awake()
    {
        // Initialize authentication and realtime database
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        dbUsersReference = FirebaseDatabase.DefaultInstance.GetReference("Users");
    }

    void Start()
    {
        // Obtain uuid of the user
        uuid = GetCurrentUser().UserId;
    }

    public FirebaseUser GetCurrentUser() // Obtain uuid of the user
    {
        return auth.CurrentUser;
    }

    // Send password reset email to user
    public async Task SendPasswordResetEmail(string email)
    {
        // If no email is provided (user not logged in or did not enter an email during login page)
        if (string.IsNullOrEmpty(email))
        {
            Debug.LogError("No email provided for password reset.");
            return;
        }
        
        try
        {
            await auth.SendPasswordResetEmailAsync(email); // Send password reset email provided by Firebase
            Debug.Log("Password reset email sent successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + ex);
        }
    }

    // Obtain the user's data 
    public async Task<Users> GetUser(string uuid)
    {
        Users users = null; // intialize empty Users object

        // Search the database
        await dbUsersReference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted) // If there is error when retrieving user's data
            {
                Debug.LogError("Sorry, there was an error retrieving player stats : ERROR " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot ds = task.Result; 
                if (ds.Child(uuid).Exists) // If the user of uuid exists in the database
                {
                    users = JsonUtility.FromJson<Users>(ds.Child(uuid).GetRawJsonValue()); // Assign the data from database to the users

                    Debug.Log("ds... : " + ds.GetRawJsonValue());
                    Debug.Log("Users values.." + users.UsersToJson());

                }
            }
        });
        return users;
    }

    // Update username data for user
    public async Task UpdateUserName(string userName)
    {
        await dbUsersReference.Child(uuid).Child("username").SetValueAsync(userName);
    }

    // Sign out the user provided by Firebase Authentication
    public void SignOutUser()
    {
        auth.SignOut();
    }

    // Update the last login time of the user
    public async Task UpdateLastLogin() 
    {
        await dbUsersReference.Child(uuid).Child("lastLogin").SetValueAsync(DateTime.Now.ToString("o")); // "o" for round-trip format of date time
    }

    // Update the last daily reward redemption time of the user
    public async Task UpdateLastRedeemed()
    {
        await dbUsersReference.Child(uuid).Child("lastRedeemed").SetValueAsync(DateTime.Now.ToString("o"));
    }

    // Increments / adds the user's daily reward redemption amount by 1 
    public async Task AddDailyRedeemed()
    {
        Users users = await GetUser(uuid);
        int userDailyRedeemed = users.dailyRedeemed;
        userDailyRedeemed++;
        await dbUsersReference.Child(uuid).Child("dailyRedeemed").SetValueAsync(userDailyRedeemed);
    }

    // Resets the user's daily reward redemption amount to 0
    public async Task ResetDailyRedeemed()
    {
        await dbUsersReference.Child(uuid).Child("dailyRedeemed").SetValueAsync(0);
    }

    // Adds the tickets and updates the daily ticket earnings 
    public async Task AddTickets(int amount)
    {
        Users users = await GetUser(uuid);
        int dailyAmount = users.dailyEarned;
        // If the user has not passed its daily ticket earning limit of 100
        if (dailyAmount <= 100)
        {
            int amountToAdd = users.tickets + amount; // Updates the total tickets of user
            int updateDailyEarned = dailyAmount + amount; // Updates the daily ticket earnings of users
            await dbUsersReference.Child(uuid).Child("tickets").SetValueAsync(amountToAdd);
            await dbUsersReference.Child(uuid).Child("dailyEarned").SetValueAsync(updateDailyEarned);
        }
    }

    // Resets the user's daily ticket earning limit to 0
    public async Task ResetDailyEarned()
    {
        await dbUsersReference.Child(uuid).Child("dailyEarned").SetValueAsync(0);
    }

    // Updates the last daily ticket earning time of the user
    public async Task UpdateLastDailyReset()
    {
        await dbUsersReference.Child(uuid).Child("lastDailyReset").SetValueAsync(DateTime.Now.ToString("o"));
    }

    // appends the voucher into the users voucher list and remove the tickets of the user
    public async Task PurchaseItem(string id, int cost)
    {
        Users users = await GetUser(uuid);
        if(id == "spinthewheel") // If the user bought a spin the wheel ticket
        {
            int spinTicketsToAdd = users.spinTickets + 1;
            await dbUsersReference.Child(uuid).Child("spinTickets").SetValueAsync(spinTicketsToAdd); // Increment / add the spin the wheel chances of the user by 1
        }
        else
        {
            List<string> vouchersRetrieved = new List<string>(); 
            vouchersRetrieved = users.vouchers; // Retrieve the list of vouchers that the user has

            foreach(var vouchers in vouchersRetrieved) // displays the vouchers of the user
            {
                Debug.Log("Current vouchers: " + vouchers);
            }

            vouchersRetrieved.Add(id); // adds the voucher's id to the list
            await dbUsersReference.Child(uuid).Child("vouchers").SetValueAsync(vouchersRetrieved); // Updates the user's voucher list with the updated list

            foreach(var vouchers in vouchersRetrieved)
            {
                Debug.Log("Added vouchers: " + vouchers); // Displays the vouchers of the user after the update
            }
        }
        
        // If the user's ticket is more than the cost
        if (users.tickets >= cost)
        {
            int amountToDeduct = users.tickets - cost;
            await dbUsersReference.Child(uuid).Child("tickets").SetValueAsync(amountToDeduct); // Update the user's ticket after the deduction
        }

    }

    // When the user uses a spin the wheel ticket
    public async Task UseSpinTicket()
    {
        Users users = await GetUser(uuid);
        int updatedSpinTickets = users.spinTickets - 1;
        await dbUsersReference.Child(uuid).Child("spinTickets").SetValueAsync(updatedSpinTickets);
    }

    // When user earns ticket without affecting daily ticket earning limit (spin the wheel / daily rewards)
    public async Task AddTicketsWithoutDaily(int amount)
    {
        Users users = await GetUser(uuid);
        int amountToAdd = users.tickets + amount;
        await dbUsersReference.Child(uuid).Child("tickets").SetValueAsync(amountToAdd); 
    }

    // Updates the last Nike trivia date of the user 
    public async Task UpdateLastNikeTrivia()
    {
        await dbUsersReference.Child(uuid).Child("lastNikeTrivia").SetValueAsync(DateTime.Now.ToString("o"));
    }

    // Check if the voucherlist has an empty slot and removes that index
    public async Task CheckIfVoucherEmpty()
    {
        Users users = await GetUser(uuid);

        List<string> newVouchers = new List<string>(); // Empty list to store new vouchers
        List<string> currentVouchers = new List<string>(); // Empty list to store old vouchers
        currentVouchers = users.vouchers; // Obtain user's old vouchers

        foreach (var voucher in currentVouchers)
        {
            if (!string.IsNullOrEmpty(voucher)) // Ignores voucher if they are null or empty
            {
                newVouchers.Add(voucher);
            }
        }
        await dbUsersReference.Child(uuid).Child("vouchers").SetValueAsync(newVouchers); // Update the user's vouchers list with new vouchers
    }
}

   


    

