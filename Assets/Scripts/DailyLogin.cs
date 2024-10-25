using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Threading.Tasks;

public class DailyLogin : MonoBehaviour
{
    private string uuid;
    public FirebaseManager fbMgr;
    public MainMenuManager mmMgr;
    public Button claimButton;
    public GameObject dailyIconBadge;
    public List<GameObject> DailyTicketIcons;
    private int dailyRedeemed;
    void Start()
    {
        uuid = fbMgr.GetCurrentUser().UserId; // Obtain uuid
        StartCoroutine(DailyLoginMethods()); // starts daily login method
    }

    IEnumerator DailyLoginMethods() // inumerator to be run at start()
    {
        CheckDailyRewardAvailability(); // Checks if daily reward can be claim
        yield return new WaitForSeconds(0.5f);
        CheckDailyEarned(); // Checks daily earned tickets
        yield return new WaitForSeconds(0.5f);
        UpdateDailyRedeemedAndGreyOut(); // update and grey out the daily reward
    }
    public void GreyOut()
    {
        // loops through daily reward collected
        for(int i = 0; i < dailyRedeemed; i++)
        {
            GameObject obj = DailyTicketIcons[i]; // daily reward ticket icons as objects
            RawImage[] images = obj.GetComponentsInChildren<RawImage>();

            // Check if any RawImage components were found
            if (images.Length == 0)
            {
                Debug.LogWarning("No RawImage components found in children of " + obj.name); 
                continue; // Skip to the next iteration
            }
            // Loops through the rawImage component found in the object
            foreach (RawImage image in images)
            {
                image.color = Color.grey; // change the image to greyish for number of daily reward collected
            }
        
        }
    }

    public async void CheckDailyRewardAvailability() 
    {
        Users users = await fbMgr.GetUser(uuid); // obtain user's data in form of class Users

        if (users.lastRedeemed != null && users.lastLogin != null)
        {
            DateTime lastRedeemedDate = DateTime.Parse(users.lastRedeemed).Date; // Get only the date part for last redeemed
            DateTime lastLoginDate = DateTime.Parse(users.lastLogin).Date; // Get only the date part for last login
            DateTime currentDate = DateTime.UtcNow.Date; // Get the current date in UTC

            if (lastRedeemedDate < currentDate) // If last redeemed date has already passed 
            {
                if (users.dailyRedeemed == 7) // When daily redeemed has completed (7 collected)
                {
                    await fbMgr.ResetDailyRedeemed(); // resets the daily reward for user
                    dailyRedeemed = 0;
                }
                claimButton.interactable = true; // Activate the claim button
                dailyIconBadge.SetActive(true); // Show icon badge

            }
            else // user already claimed on that day
            {
                claimButton.interactable = false; // Deactivate the claim button 
                dailyIconBadge.SetActive(false); // do not show icon badge
            }
        }
    }

    public async void ClaimDailyReward() // When user claim daily reward
    {
        claimButton.interactable = false;
        Users users = await fbMgr.GetUser(uuid);
        await fbMgr.UpdateLastRedeemed(); // update last daily reward redemption time
        await fbMgr.AddDailyRedeemed(); // Increment the daily reward redeemed number

        await UpdateDailyRedeemedAndGreyOut(); // Call function to get latest updated daily reward and grey out.
        await fbMgr.AddTicketsWithoutDaily(dailyRedeemed*5); // Add tickets without affecting daily ticket limit
        mmMgr.DisplayMainMenuInfo(); // Update user's tickets in main menu
        dailyIconBadge.SetActive(false);
    }

    private async Task UpdateDailyRedeemedAndGreyOut()
    {
        // Get the updated daily redeemed count
        await GetDailyRedeemed();

        // Grey out the appropriate elements using the updated value
        GreyOut();
    }


    public async void CheckDailyEarned() 
    {
        Users users = await fbMgr.GetUser(uuid);
        DateTime lastDailyResetDate = DateTime.MinValue; // Assigns a minimum date when user have not had their first daily ticket limit reset

        if (!string.IsNullOrEmpty(users.lastDailyReset))
        {
            lastDailyResetDate = DateTime.Parse(users.lastDailyReset).Date; // Obtain last daily ticket limit reset
        }

        DateTime currentDate = DateTime.UtcNow.Date;
        
        if (lastDailyResetDate < currentDate) // If at least 1 day has passed since reset
        {
            // Reset daily earned amount and update the last daily reset date
            await fbMgr.ResetDailyEarned();
            await fbMgr.UpdateLastDailyReset();
        }
    }

    public async Task GetDailyRedeemed() // Update daily reward redeem
    {
        Users user = await fbMgr.GetUser(uuid);
        dailyRedeemed = user.dailyRedeemed;
    }
}
