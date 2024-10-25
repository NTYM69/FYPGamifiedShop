using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Users // Class for user's data structure
{
    public string username;
    public int tickets;
    public int spinTickets;
    public int dailyRedeemed;
    public int dailyEarned;
    public string lastLogin;
    public string lastDailyReset;
    public string lastRedeemed;
    public string lastNikeTrivia;
    public List<string> vouchers;

   public Users(string username = "", int tickets = 0, int spinTickets = 0, int dailyRedeemed = 0, int dailyEarned = 0, DateTime? lastLogin = null, DateTime? lastDailyReset = null, DateTime? lastRedeemed = null, DateTime? lastNikeTrivia = null, List<string> vouchers = null)
   {
        this.username = username;
        this.tickets = tickets;
        this.spinTickets = spinTickets;
        this.dailyRedeemed = dailyRedeemed;
        this.dailyEarned = dailyEarned;
        this.lastLogin = lastLogin?.ToString("o") ?? DateTime.MinValue.ToString("o");
        this.lastDailyReset = lastDailyReset?.ToString("o");
        this.lastRedeemed = lastRedeemed?.ToString("o");
        this.lastNikeTrivia = lastNikeTrivia?.ToString("o");
        this.vouchers = vouchers ?? new List<string>();
   }

    public string UsersToJson()
    {
        return JsonUtility.ToJson(this); // Convert users class data to json format
    }
}
