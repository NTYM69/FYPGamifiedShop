using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpinTheWheelManager : MonoBehaviour
{
    public FirebaseManager fbMgr;
    [SerializeField] private TMP_Text spinNo, itemName;
    [SerializeField] private GameObject SpinTheWheelCanvas;
    public SpinRewardTemplate[] spinRewards;
    [SerializeField] private Image itemImage;
    public Button spinButton;
    private string uuid;
    private Users users;
    private float spinPower;
    private float stopPower;
    public Rigidbody2D rbody;
    float t;
    bool inRotate;
    void Start()
    {
        uuid = fbMgr.GetCurrentUser().UserId;
        displaySpinTicket();
        SpinTheWheelCanvas.SetActive(false); // Disable the spin the wheel popup
    }

    void Update()
    {
        CheckSpinnable();

        if (rbody.angularVelocity > 0)
        {
            rbody.angularVelocity -= stopPower * Time.deltaTime; // Slows down the rigidbody in rotation based on stopPower
            rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, 0, 1440); // Ensures that the rigidbody is within the range of 0 - 1440
        }

        // When wheel stops rotating and has been rotated
        if (rbody.angularVelocity == 0 && inRotate == true)
        {
            t += 1*Time.deltaTime; // Start counting the time 
            if(t>= 0.5f) // after 0.5s get reward and set wheel in rotation to false
            {
                GetReward();

                inRotate = false;
                t = 0; // reset time
            }
        }
    }
    public async void displaySpinTicket()
        {
            // displays the user's spin the wheel tickets 
            users = await fbMgr.GetUser(uuid);
            spinNo.text = users.spinTickets.ToString();
        }

    public async void RotateWheel()
    {
        // Randomly generate spinPower and stopPower relative to spinPower
        spinPower = Random.Range(700, 4000);
        stopPower = spinPower / Random.Range(2,8);

        if (inRotate == false) // If wheel is not rotating
        {
            rbody.AddTorque(spinPower); // Spin the rigidbody according to spinPower
            inRotate = true; // Set the wheel in rotation to true
        }

        await fbMgr.UseSpinTicket(); // Updates the database of the user 
        displaySpinTicket(); // Display new spin the wheel tickets
    }

    //Handles reward screen
    public async void HandleReward(int rewardNo)
    {
        SpinTheWheelCanvas.SetActive(true); // Enable the reward popup
        itemName.text = spinRewards[rewardNo].itemName; // Obtain item name
        itemImage.sprite = spinRewards[rewardNo].itemImage.sprite; // Obtain item image

        if (spinRewards[rewardNo].isTicket == true) // If the reward is tickets, add to user's total ticket
        {
            await fbMgr.AddTicketsWithoutDaily(spinRewards[rewardNo].ticketAmount);
        }
        else
        {
            await fbMgr.PurchaseItem(spinRewards[rewardNo].itemId, 0); // If the ticket is a voucher, add to user's vouchers
        }
    }

    public void GetReward()
    {
        float rotation = rbody.transform.eulerAngles.z; // Obtain the current rotation of the wheel in the range of 360 degrees
        Debug.Log("Rotation is : " + rotation);
        
        // -18 is used as the wheel starts at the center of the first slot, making is offset by 18 on both positive and negative rotation
        if (rotation > 360-18 || rotation <= 36-18) // If reward is in the first slot
        {
            HandleReward(0); 
        }
        else if (rotation > 36-18 && rotation <= 72-18) // If reward is in the second slot
        {
            HandleReward(1);
        }
        else if (rotation > 72-18 && rotation <= 108-18) // If reward is in the third slot
        {
            HandleReward(2);
        }
        else if (rotation > 108-18 && rotation <= 144-18) // If reward is in the fourth slot
        {
            HandleReward(3);
        }
        else if (rotation > 144-18 && rotation <= 180-18) // If reward is in the fifth slot
        {
            HandleReward(4);
        }
        else if (rotation > 180-18 && rotation <= 216-18) // If reward is in the sixth slot
        {
            HandleReward(5);
        }
        else if (rotation > 216-18 && rotation <= 252-18) // If reward is in the seventh slot
        {
            HandleReward(6);
        }
        else if (rotation > 252-18 && rotation <= 286-18) // If reward is in the eighth slot
        {
            HandleReward(7);
        }
        else if (rotation > 286-18 && rotation <= 322-18) // If reward is in the ninth slot 
        {
            HandleReward(8);
        }
        else if (rotation > 322-18 && rotation <= 360-18)// If reward is in the tenth slot 
        {
            HandleReward(9);
        }
    }

    public void CloseSpinPopup()
    {
        // Close popup and reset the wheel to its original rotation
        SpinTheWheelCanvas.SetActive(false);
        rbody.transform.rotation = Quaternion.identity;
    }

    public void CheckSpinnable()
    {
        // Skip checking if user has not yet been retrieved
        if (users == null)
        {
            Debug.LogWarning("Users object is null. Skipping CheckPurchasable.");
            return;
        }

        // Enable button if user has at least 1 ticket
        if (users.spinTickets >= 1)
        {
            spinButton.interactable = true;
        }
        else
        {
            spinButton.interactable = false;
        }
    }
   
}
