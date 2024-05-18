using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Users
{
    public string username;
    public int tickets;
    public int spinTickets;
    public int dailyRedeemed;
    public int dailyEarned;
    public string lastLogin;
    public string lastRedeemed;
    public List<string> vouchers;

   public Users(string username = "", int tickets = 0, int spinTickets = 0, int dailyRedeemed = 0, int dailyEarned = 0, DateTime? lastLogin = null, DateTime? lastRedeemed = null, List<string> vouchers = null){
        this.username = username;
        this.tickets = tickets;
        this.spinTickets = spinTickets;
        this.dailyRedeemed = dailyRedeemed;
        this.lastLogin = lastLogin?.ToString("o") ?? DateTime.MinValue.ToString("o"); // ISO 8601 format
        this.lastRedeemed = lastRedeemed?.ToString("o");
        this.vouchers = vouchers ?? new List<string>();
   }

    public string UsersToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
