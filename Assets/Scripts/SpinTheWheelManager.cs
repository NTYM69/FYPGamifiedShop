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
        SpinTheWheelCanvas.SetActive(false);
    }

    void Update()
    {
        CheckSpinnable();

        if (rbody.angularVelocity > 0)
        {
            rbody.angularVelocity -= stopPower * Time.deltaTime;
            rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, 0, 1440);
        }

        if (rbody.angularVelocity == 0 && inRotate == true)
        {
            t += 1*Time.deltaTime;
            if(t>= 0.5f)
            {
                GetReward();

                inRotate = false;
                t = 0;
            }
        }
    }
    public async void displaySpinTicket()
        {
            users = await fbMgr.GetUser(uuid);
            spinNo.text = users.spinTickets.ToString();
        }

    public async void RotateWheel()
    {
        spinPower = Random.Range(700, 4000);
        stopPower = spinPower / Random.Range(2,8);

        Debug.Log("Spin power & Stop power: " + spinPower + ", " + stopPower);
        if (inRotate == false)
        {
            rbody.AddTorque(spinPower);
            inRotate = true;
        }

        await fbMgr.UseSpinTicket();
        displaySpinTicket();
    }
    public async void HandleReward(int rewardNo)
    {
        Debug.Log("You got reward "+rewardNo);
        SpinTheWheelCanvas.SetActive(true);
        itemName.text = spinRewards[rewardNo].itemName;
        itemImage.sprite = spinRewards[rewardNo].itemImage.sprite;

        if (spinRewards[rewardNo].isTicket == true)
        {
            await fbMgr.AddTicketsWithoutDaily(spinRewards[rewardNo].ticketAmount);
        }
        else
        {
            await fbMgr.PurchaseItem(spinRewards[rewardNo].itemId, 0);
        }
    }

    public void GetReward()
    {
        float rotation = rbody.transform.eulerAngles.z;
        Debug.Log("Rotation is : " + rotation);
        
        if (rotation > 360-18 || rotation <= 36-18)
        {
            HandleReward(0);
        }
        else if (rotation > 36-18 && rotation <= 72-18)
        {
            HandleReward(1);
        }
        else if (rotation > 72-18 && rotation <= 108-18)
        {
            HandleReward(2);
        }
        else if (rotation > 108-18 && rotation <= 144-18)
        {
            HandleReward(3);
        }
        else if (rotation > 144-18 && rotation <= 180-18)
        {
            HandleReward(4);
        }
        else if (rotation > 180-18 && rotation <= 216-18)
        {
            HandleReward(5);
        }
        else if (rotation > 216-18 && rotation <= 252-18)
        {
            HandleReward(6);
        }
        else if (rotation > 252-18 && rotation <= 286-18)
        {
            HandleReward(7);
        }
        else if (rotation > 286-18 && rotation <= 322-18)
        {
            HandleReward(8);
        }
        else if (rotation > 322-18 && rotation <= 360-18)
        {
            HandleReward(9);
        }
    }

    public void CloseSpinPopup()
    {
        SpinTheWheelCanvas.SetActive(false);
        rbody.transform.rotation = Quaternion.identity;
    }

    public void CheckSpinnable()
    {
        if (users == null)
        {
            Debug.LogWarning("Users object is null. Skipping CheckPurchasable.");
            return;
        }

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
