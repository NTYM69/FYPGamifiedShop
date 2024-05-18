using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public FirebaseManager fbMgr;
    [SerializeField] private TMP_Text ProfileUsername, TicketNo;
    private string uuid;
    void Start()
    {
        Application.targetFrameRate = ((int)Screen.currentResolution.refreshRateRatio.value);
        uuid = fbMgr.GetCurrentUser().UserId;
        DisplayMainMenuInfo(uuid);
        CheckLogin();
    }
    private async void DisplayMainMenuInfo(string uuid) 
    {
        Users users = await fbMgr.GetUser(uuid);
        ProfileUsername.text = users.username;
        TicketNo.text = users.tickets.ToString();
        Debug.Log("username is " + users.username);
    }

    public async void CheckLogin() {
        Users users = await fbMgr.GetUser(uuid);
        fbMgr.UpdateLastLogin(uuid);
    }


}
