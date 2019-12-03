using UnityEngine;
using UnityEngine.UI;

public class UITechCategoryButton : MonoBehaviour
{
    public Text buttonText;
    public UITechMenu techMenu;
    public string category;

    public void DoSelect()
    {
        techMenu.InitializeObjectMenu(category);
    }
}
