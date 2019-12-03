using UnityEngine;
using UnityEngine.UI;

public class UITechObjectButton : MonoBehaviour
{
    public Text buttonText;
    public UITechMenu techMenu;
    public string objectName;

    public void DoSelect()
    {
        techMenu.upgradeClassName = objectName.ToLower();
        techMenu.InitializeSpecificUpgradesMenu(objectName);
    }
}
