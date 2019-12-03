using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    public void Open()
    {
        HexGameUI.menuOpen = true;
        gameObject.SetActive(true);
        HexMapCamera.Locked = true;
    }

    public void Close()
    {
        HexGameUI.menuOpen = false;
        gameObject.SetActive(false);
        HexMapCamera.Locked = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
