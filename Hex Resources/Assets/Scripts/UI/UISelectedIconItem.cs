using UnityEngine;
using UnityEngine.UI;

public class UISelectedIconItem : MonoBehaviour
{
    public HexGameUI hexGameUI;

    public string actionName;

    public Text itemLabel;

    public void DoAction()
    {
        UISelectedIconItem actionItem = this;
        hexGameUI.DoSelectedAction(actionItem);
    }

    public void SetFields(string actionName)
    {
        this.actionName = actionName;
        itemLabel.text = actionName;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
