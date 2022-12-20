using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject start_menu;
    public TMP_InputField username;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying objects...");
            Destroy(this);
        }
    }

    public void ConnectToServer()
    {
        start_menu.SetActive(false);
        username.interactable = false;
        Client.instance.ConnectToServer();
    }
}
