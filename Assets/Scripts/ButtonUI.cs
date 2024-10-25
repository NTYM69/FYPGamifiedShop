using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] GameObject dailyRewardCanvas;
    [SerializeField] GameObject myVouchersCanvas;

    void Start() {
        // close daily reward canvas
        dailyRewardCanvas.SetActive(false);
    }
    public void LoginButton() {
        SceneManager.LoadScene(1); // navigate to login screen
    }

    public void RegisterButton() {
        SceneManager.LoadScene(2); // navigate to register screen
    }

    public void backLandingButton() {
        SceneManager.LoadScene(0); // navigate to main landing page
    }

    public void backMainMenu() {
        SceneManager.LoadScene(3); // navigate to main menu
    }

    public void goProfilePage() {
        SceneManager.LoadScene(4); // navigate to profile page
    }

    public void goShopPage() {
        SceneManager.LoadScene(5); // navigate to shop page
    }

    public void playFrogGame() {
        SceneManager.LoadScene(6); // navigate to river dash game 
    }

    public void openDailyLogin() {
        dailyRewardCanvas.SetActive(true); // Open daily login reward popup
    }


    public void CloseDailyLogin() {
        dailyRewardCanvas.SetActive(false); // Closes daily login reward popup
    }

    public void openVouchers()
    {
        myVouchersCanvas.SetActive(true); // Open vouchers popup
    }
    public void closeVouchers()
    {
        myVouchersCanvas.SetActive(false); // Close vouchers popup
    }

    public void GoSpinTheWheel()
    {
        SceneManager.LoadScene(7); // Navigate to spin the wheel page
    }
    public void GoTrivia()
    {
        SceneManager.LoadScene(8); // Navigate to trivia page
    }

}
