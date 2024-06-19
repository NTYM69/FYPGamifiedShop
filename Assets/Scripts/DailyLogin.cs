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
    // Start is called before the first frame update
    public Button claimButton;
    public List<GameObject> DailyTicketIcons;
    public int dailyRedeemed;
    void Start()
    {
        uuid = fbMgr.GetCurrentUser().UserId;
        StartCoroutine(DailyLoginMethods());
        // CheckDailyRewardAvailability();
        // CheckDailyEarned();
        // UpdateDailyRedeemedAndGreyOut();
        Debug.Log("daily redeemed is " + dailyRedeemed);
        // GreyOut();
        
        Debug.Log("count: " + DailyTicketIcons.Count);
    }

    IEnumerator DailyLoginMethods()
    {
        CheckDailyRewardAvailability();
        yield return new WaitForSeconds(0.5f);
        CheckDailyEarned();
        yield return new WaitForSeconds(0.5f);
        UpdateDailyRedeemedAndGreyOut();
    }
    public void GreyOut()
    {
        // GetDailyRedeemed();

        for(int i = 0; i < dailyRedeemed; i++)
        {
            GameObject obj = DailyTicketIcons[i];
            Debug.Log("Attempting to grey out " + obj.name);

            Debug.Log("Attempting");
            RawImage[] images = obj.GetComponentsInChildren<RawImage>();

            // Check if any RawImage components were found
            if (images.Length == 0)
            {
                Debug.LogWarning("No RawImage components found in children of " + obj.name);
                continue; // Skip to the next iteration
            }

            foreach (RawImage image in images)
            {
                Debug.Log("Entered the loop");
                image.color = Color.grey;
            }
        
        }
    }
    public async void CheckDailyRewardAvailability() 
    {
        Users users = await fbMgr.GetUser(uuid);

        if (users.lastRedeemed != null && users.lastLogin != null)
        {
            DateTime lastRedeemedDate = DateTime.Parse(users.lastRedeemed).Date; // Get only the date part
            DateTime lastLoginDate = DateTime.Parse(users.lastLogin).Date; // Get only the date part
            DateTime currentDate = DateTime.UtcNow.Date; // Get the current date in UTC

            if (lastRedeemedDate < lastLoginDate || lastRedeemedDate < currentDate)
            {
                // int userDailyRedeem = await fbMgr.CheckDailyRedeemed(uuid);
                if (users.dailyRedeemed == 7) 
                {
                    await fbMgr.ResetDailyRedeemed(uuid);
                    dailyRedeemed = 0;
                }
                //activate button
                Debug.Log("YOU CAN REDEEM THE DAILY LOGIN");
                claimButton.interactable = true;
            }
            else
            {
                //deactive button
                Debug.Log("YOU CANNOT!");
                claimButton.interactable = false;
            }
        }
    }

    public async void ClaimDailyReward() 
    {
        Users users = await fbMgr.GetUser(uuid);
        await fbMgr.UpdateLastRedeemed(uuid);
        await fbMgr.AddDailyRedeemed(uuid);

        await UpdateDailyRedeemedAndGreyOut();
        await fbMgr.AddTicketsWithoutDaily(dailyRedeemed);
        mmMgr.DisplayMainMenuInfo(uuid);
        claimButton.interactable = false;
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

        DateTime lastDailyResetDate = DateTime.MinValue;

        if (!string.IsNullOrEmpty(users.lastDailyReset))
        {
            lastDailyResetDate = DateTime.Parse(users.lastDailyReset).Date;
        }
        // DateTime lastDailyResetDate = DateTime.Parse(users.lastDailyReset).Date;
        Debug.Log("last reset date " + lastDailyResetDate);
        
        DateTime currentDate = DateTime.UtcNow.Date;
        Debug.Log("current date " + currentDate);
        
        if (string.IsNullOrEmpty(users.lastDailyReset) || lastDailyResetDate < currentDate)
        {
            // Reset daily earned amount and update the last daily reset date
            await fbMgr.ResetDailyEarned(uuid);
            await fbMgr.UpdateLastDailyReset(uuid);
        }
    }

    public async Task GetDailyRedeemed()
    {
        Users user = await fbMgr.GetUser(uuid);
        dailyRedeemed = user.dailyRedeemed;
        Debug.Log("daily redeemed: " + dailyRedeemed);

        // callback.Invoke();

    }
}
