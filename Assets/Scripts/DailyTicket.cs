using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DailyTicket : MonoBehaviour
{
    [SerializeField] private TMP_Text ticketAmount, dayNo;
    public FirebaseManager fbMgr;
    public int day;

    // Start is called before the first frame update
    void Start()
    {
        ticketAmount.text = day.ToString();
        dayNo.text = day.ToString();
    }   
}
