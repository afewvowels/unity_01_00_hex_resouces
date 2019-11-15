using UnityEngine;

public class UIGameMenu : MonoBehaviour
{
    public void Open()
    {
        HexMapCamera.Locked = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        HexMapCamera.Locked = false;
        gameObject.SetActive(false);
    }
}
