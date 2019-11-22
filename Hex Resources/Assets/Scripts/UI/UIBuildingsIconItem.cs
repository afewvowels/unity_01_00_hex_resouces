using UnityEngine;
using UnityEngine.UI;

public class UIBuildingsIconItem : MonoBehaviour
{
    public UIBuildingsMenu buildingsMenu;

    [SerializeField]
    private BuildingBaseClass buildingInfo;
    public UISelectedMenu selectedMenu;
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
        if (buildingsMenu)
        {
            buildingsMenu.SelectMenuItem(buildingInfo);
        }
        else if (selectedMenu)
        {
            Debug.Log("place building initiated");
            StopAllCoroutines();
            StartCoroutine(selectedMenu.PlaceBuilding(buildingInfo));
        }
        else
        {
            Debug.Log("No menu associated with this button");
        }
    }

    public void SetFields(BuildingBaseClass building)
    {
        BuildingInfo = building;
        thisBuildingName = building.buildingName;
        transform.GetChild(0).GetComponent<Text>().text = thisBuildingName;
        buildingID = building.BuildingID;
    }
}
