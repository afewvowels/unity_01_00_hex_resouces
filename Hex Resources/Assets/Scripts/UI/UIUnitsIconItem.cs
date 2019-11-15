using UnityEngine;
using UnityEngine.UI;

public class UIUnitsIconItem : MonoBehaviour
{
    public UIUnitsMenu unitsMenu;

    [SerializeField]
    private HexUnit unitInfo;

    [SerializeField]
    private string thisUnitName;

    //public string UnitName
    //{
    //    get
    //    {
    //        return unitInfo.unitName;
    //    }
    //    set
    //    {
    //        unitInfo.unitName = value;
    //    }
    //}

    public HexUnit UnitInfo
    {
        get
        {
            return unitInfo;
        }
        set
        {
            unitInfo = value;
        }
    }

    public void Select()
    {
        unitsMenu.SelectMenuItem(unitInfo);
    }

    public void SetFields(HexUnit unit)
    {
        UnitInfo = unit;
        thisUnitName = unit.unitName;
        transform.GetChild(0).GetComponent<Text>().text = thisUnitName;
    }
}
