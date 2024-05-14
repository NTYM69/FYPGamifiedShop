using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private TMP_Text TicketNo;
    public FirebaseManager fbMgr;
    private string uuid;

    void Start() 
    {
        uuid = fbMgr.GetCurrentUser().UserId;
        DisplayTicketNo(uuid);
    }

    public async void DisplayTicketNo(string uuid) {
        Users users = await fbMgr.GetUser(uuid);
        TicketNo.text = users.tickets.ToString();
    }

}
