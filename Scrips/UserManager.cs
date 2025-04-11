using UnityEngine;

public class UserManager : MonoBehaviour
{
    public GameObject registerPanel;
    public GameObject loginPanel;

    public void OpenRegisterPanel()
    {
        registerPanel.SetActive(true);
        loginPanel.SetActive(false);
    }
    public void OpenLogin()
    {
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
    }
}