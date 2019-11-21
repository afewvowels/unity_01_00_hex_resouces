using UnityEngine;
using UnityEngine.UI;

public class UIBuildingsIconItem : MonoBehaviour
{
    public UIBuildingsMenu buildingsMenu;

    [SerializeField]
    private BuildingBaseClass buildingInfo;

    [SerializeField]
    private string thisBuildingName;

    [SerializeField]
    private int buildingID;

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

    public BuildingBaseClass BuildingInfo
    {
        get
        {
            return buildingInfo;
        }
        set
        {
            buildingInfo = value;
        }
    }

    public void Select()
    {
        buildingsMenu.SelectMenuItem(buildingInfo);
    }

    public void SetFields(BuildingBaseClass building)
    {
        BuildingInfo = building;
        thisBuildingName = building.buildingName;
        transform.GetChild(0).GetComponent<Text>().text = thisBuildingName;
        buildingID = building.BuildingID;
    }
}
