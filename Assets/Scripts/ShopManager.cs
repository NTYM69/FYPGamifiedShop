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
        // users = fbMgr.GetUser(uuid);
        DisplayTicketNo(uuid);
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            shopPanelsGO[i].SetActive(true);
        }
        LoadPanels();
    }
    void Update()
    {
        CheckPurchasable();
    }

    public async void DisplayTicketNo(string uuid) 
    {
        users = await fbMgr.GetUser(uuid);
        TicketNo.text = users.tickets.ToString();
    }

    public void LoadPanels()
    {
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            shopPanels[i].itemTitle.text = shopItemsSO[i].title;
            shopPanels[i].itemCost.text = shopItemsSO[i].cost.ToString();

            Sprite itemSprite = Resources.Load<Sprite>($"ShopItemImages/{shopItemsSO[i].id}");
            if (itemSprite != null)
            {
                shopPanels[i].itemImage.sprite = itemSprite;
            }
            else
            {
                Debug.Log($"Error, image not found for {shopItemsSO[i].id}!");
            }
        }
    }

    public void CheckPurchasable()
    {
         if (users == null)
        {
            Debug.LogWarning("Users object is null. Skipping CheckPurchasable.");
            return;
        }

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

    public async void PurchaseItem(int btnNo)
    {
        if (users.tickets >= shopItemsSO[btnNo].cost)
        {
            // handle purchasing here
            await fbMgr.PurchaseItem(shopItemsSO[btnNo].id, shopItemsSO[btnNo].cost);
        }
        DisplayTicketNo(uuid);
    }
}
