using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWelcomeMenu : MonoBehaviour
{
    public void Close()
    {
        gameObject.SetActive(false);
        HexGameUI.menuOpen = false;
    }
}
