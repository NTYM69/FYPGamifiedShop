using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameData 
{
    public string username;
    public int tickets;
    public int spinTickets;
    public int dailyRedeemed;
    public int dailyEarned;
    public DateTime lastLogin = DateTime.MinValue;
    public List<string> vouchers;

   public GameData(string username = "", int tickets = 0, int spinTickets = 0, int dailyRedeemed = 0, int dailyEarned = 0, DateTime? lastLogin = null, List<string> vouchers = null){
        this.username = username;
        this.tickets = tickets;
        this.spinTickets = spinTickets;
        this.dailyRedeemed = dailyRedeemed;
        this.lastLogin = lastLogin ?? DateTime.MinValue;
        this.vouchers = vouchers ?? new List<string>();
   }
}
