using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISecretMenu : MonoBehaviour
{
    public bool isOpen;

    GameObject[] buttons;

    private void Start()
    {
        buttons = GameObject.FindGameObjectsWithTag("secret");
    }

    public void Open()
    {
        buttons[0].SetActive(true);
        buttons[1].SetActive(true);
        isOpen = true;
    }

    public void Close()
    {
        buttons[0].SetActive(false);
        buttons[1].SetActive(false);
        isOpen = false;
    }
}
