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
    void Start()
    {
        Application.targetFrameRate = ((int)Screen.currentResolution.refreshRateRatio.value);
        uuid = fbMgr.GetCurrentUser().UserId;
        DisplayMainMenuInfo(uuid);
        CheckLogin();
        LoadVoucherPanels();
    }
    public async void DisplayMainMenuInfo(string uuid) 
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

    public async void LoadVoucherPanels()
    {
        Users users = await fbMgr.GetUser(uuid);
        List<string> vouchersRetrieved = new List<string>();
        vouchersRetrieved = users.vouchers;

        for (int i = 0; i < vouchersRetrieved.Count; i++)
        {
            voucherPanelsGO[i].SetActive(true);
            Sprite itemSprite = Resources.Load<Sprite>($"ShopItemImages/{vouchersRetrieved[i]}");
            if (itemSprite != null)
            {
                voucherPanels[i].itemImage.sprite = itemSprite;
            }
            else
            {
                Debug.Log($"Error, image not found for {vouchersRetrieved[i]}!");
            }
        }
    }



}
