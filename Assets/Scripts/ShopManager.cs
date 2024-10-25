using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private TMP_Text TicketNo;
    public FirebaseManager fbMgr;
    private string uuid;
    public ShopItemSO[] shopItemsSO;
    public ShopTemplate[] shopPanels;
    public GameObject[] shopPanelsGO;
    public Button[] buyButton; 
    public Users users;

    void Start() 
    {
        uuid = fbMgr.GetCurrentUser().UserId;
        DisplayTicketNo();

        // Activate number of shop panels based on number of scriptable objects
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            shopPanelsGO[i].SetActive(true);
        } 
        LoadPanels();
    }

    void Update()
    {
        // Constantly check if the user can purchase the items
        CheckPurchasable(); 
    }

    public async void DisplayTicketNo() 
    {
        users = await fbMgr.GetUser(uuid);
        TicketNo.text = users.tickets.ToString();
    }

    public void LoadPanels()
    {
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            // Sets the shop panel's name and cost to the corresponding scriptable object 
            shopPanels[i].itemTitle.text = shopItemsSO[i].title;
            shopPanels[i].itemCost.text = shopItemsSO[i].cost.ToString();

            Sprite itemSprite = Resources.Load<Sprite>($"ShopItemImages/{shopItemsSO[i].id}"); // Obtain image for the scriptable object
            if (itemSprite != null)
            {
                shopPanels[i].itemImage.sprite = itemSprite; // Setst he shop panel's image to the corresponding image
            }
            else
            {
                // If no image was found 
                Debug.Log($"Error, image not found for {shopItemsSO[i].id}!");
            }
        }
    }

    public void CheckPurchasable()
    {
        // If the users have not been retrieved yet
        if (users == null)
        {
            Debug.LogWarning("Users object is null. Skipping CheckPurchasable.");
            return;
        }

        // Check if user can purchase the items
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            if (users.tickets >= shopItemsSO[i].cost)
            {
                buyButton[i].interactable = true;
            }
            else
            {
                buyButton[i].interactable = false;
            }
        }
    }

    // When user purchase the item from the shop
    public async void PurchaseItem(int btnNo)
    {
        // If user has enough or more ticket than the shop item
        if (users.tickets >= shopItemsSO[btnNo].cost)
        {
            // handle purchasing here
            await fbMgr.PurchaseItem(shopItemsSO[btnNo].id, shopItemsSO[btnNo].cost);
        }
        DisplayTicketNo(); // Update the new ticket after purchasing
    }
}
