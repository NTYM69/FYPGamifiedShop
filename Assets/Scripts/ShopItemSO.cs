using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopMenu", menuName = "Scriptable Objects/New Shop Item", order=1)] // Adds shortcut to create new scriptable object
public class ShopItemSO : ScriptableObject
{
    public string title;
    public string id;
    public int cost;
    
}
