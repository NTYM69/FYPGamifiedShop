using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DailyLogin : MonoBehaviour
{
    private string uuid;
    public FirebaseManager fbMgr;
    // Start is called before the first frame update
    public Button claimButton;
    void Start()
    {
         uuid = fbMgr.GetCurrentUser().UserId;
         CheckDailyRewardAvailability();
         CheckDailyEarned();
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
                    fbMgr.ResetDailyRedeemed(uuid);
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
        fbMgr.UpdateLastRedeemed(uuid);
        fbMgr.AddDailyRedeemed(uuid);
        claimButton.interactable = false;
    }

    public async void CheckDailyEarned()
    {
        Users users = await fbMgr.GetUser(uuid);
        DateTime lastDailyResetDate = DateTime.Parse(users.lastDailyReset).Date;
        DateTime currentDate = DateTime.UtcNow.Date;

        if (users.lastDailyReset == null || lastDailyResetDate < currentDate)
        {
            fbMgr.ResetDailyEarned(uuid);
            fbMgr.UpdateLastDailyReset(uuid);
        }
    }
}
