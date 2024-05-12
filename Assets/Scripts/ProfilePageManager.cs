using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfilePageManager : MonoBehaviour
{
    public FirebaseManager fbMgr;
    [SerializeField] private TMP_InputField editUserName;
    private string currentUserName;
    private string uuid;

    // Start is called before the first frame update
    void Start()
    {
        uuid = fbMgr.GetCurrentUser().UserId;
        DisplayUserName(uuid);
    }

    private async void DisplayUserName(string uuidDUN)
    {
        Users users = await fbMgr.GetUser(uuidDUN);
        currentUserName = users.username;
        editUserName.text = currentUserName;
        Debug.Log(currentUserName);
    }

    public async void UpdateUserName()
    {

        Users users = await fbMgr.GetUser(uuid);
        currentUserName = editUserName.text;
        fbMgr.UpdateUserName(uuid, editUserName.text);

    }
   
}
