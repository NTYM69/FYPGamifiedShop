using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public FirebaseManager fbMgr;
    [SerializeField] private TMP_Text ProfileUsername, TicketNo;
    private string uuid;
    public VoucherTemplate[] voucherPanels;
    public GameObject[] voucherPanelsGO;
    public GameObject dailyTicketWarning;
    private Users users;
    void Start()
    {
        // Sets the application framerate to the users' refresh rate
        Application.targetFrameRate = ((int)Screen.currentResolution.refreshRateRatio.value);
        uuid = fbMgr.GetCurrentUser().UserId; // Obtain uuid of the user
        
        // Calls the necessary function on arriving to main menu
        DisplayMainMenuInfo();
        CheckDailyEarned();
        CheckLogin();
        LoadVoucherPanels();  
    }

    public async void DisplayMainMenuInfo() 
    {
        // Updates the current tickets and username for the user
        users = await fbMgr.GetUser(uuid);
        ProfileUsername.text = users.username;
        TicketNo.text = users.tickets.ToString();
    }

    public async void CheckLogin() {
        // updates the last login for the user
        Users users = await fbMgr.GetUser(uuid);
        await fbMgr.UpdateLastLogin();
    }

    public async void LoadVoucherPanels()
    {
        // First checks if the user has empty elements in their vouchers list and removes said elements
        Users users = await fbMgr.GetUser(uuid);
        await fbMgr.CheckIfVoucherEmpty();

        users = await fbMgr.GetUser(uuid); // Obtain updated vouchers list

        List<string> vouchersRetrieved = new List<string>(); // Initialise list to store the user's vouchers
        vouchersRetrieved = users.vouchers; // Obtain the user's vouchers

        for (int i = 0; i < vouchersRetrieved.Count; i++) // Loops through the user's vouchers
        {
            // Sets the number of panels to active according to the vouchers 
            voucherPanelsGO[i].SetActive(true);
            Sprite itemSprite = Resources.Load<Sprite>($"ShopItemImages/{vouchersRetrieved[i]}"); // Obtain the image for the voucher
            if (itemSprite != null)
            {
                voucherPanels[i].itemImage.sprite = itemSprite; // Updates the voucher panel with the corresponding image
            }
            else
            {   // No image found
                Debug.Log($"Error, image not found for {vouchersRetrieved[i]}!"); 
            }
        }
    }
    public async void CheckDailyEarned()
    {
        // Display warning to user if they have reached their daily ticket earning limit of 100 
        Users users = await fbMgr.GetUser(uuid);
        if (users.dailyEarned >= 100)
        {
            dailyTicketWarning.SetActive(true);
        }
        else
        {
            dailyTicketWarning.SetActive(false);
        }
    }
}
