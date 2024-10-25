using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DailyTicket : MonoBehaviour
{
    [SerializeField] private TMP_Text ticketAmount, dayNo;
    public FirebaseManager fbMgr;
    public int day;

    void Start()
    {
        ticketAmount.text = (day*5).ToString(); // display ticket amount
        dayNo.text = day.ToString(); // display day
    }   
}
