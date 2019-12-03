using UnityEngine;
using UnityEngine.UI;

public class UIBuildingsIconItem : MonoBehaviour
{
    public UIBuildingsMenu buildingsMenu;

    [SerializeField]
    private BuildingBaseClass buildingInfo;
    public HexGameUI hexGameUI;
    [SerializeField]
    private string thisBuildingName;

    [SerializeField]
    private int buildingID;

    private BuildingHub buildingHub;

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

    public BuildingHub HubBuilding
    {
        get
        {
            return buildingHub;
        }
        set
        {
            buildingHub = value;
        }
    }

    public void Select()
    {
        //if (buildingsMenu)
        //{
        //    buildingsMenu.SelectMenuItem(buildingInfo);
        //}
        //else if (hexGameUI)
        //{
        HexGameUI.placeBuilding = true;
        StopAllCoroutines();
        //StartCoroutine(hexGameUI.PlaceBuilding(buildingInfo));

        if (buildingInfo.GetComponent<BuildingHub>())
        {
            StartCoroutine(hexGameUI.PlaceHubBuilding(buildingInfo));
        }
        else
        {
            StartCoroutine(hexGameUI.PlaceHubAddOnBuilding(HubBuilding, buildingInfo));
        }
        //}
        //else
        //{
        //    Debug.Log("No menu associated with this button");
        //}
    }

    public void SetFields(BuildingBaseClass building)
    {
        BuildingInfo = building;
        thisBuildingName = building.buildingName;
        transform.GetChild(0).GetComponent<Text>().text = thisBuildingName;
        buildingID = building.BuildingID;
    }

    //public void SetFields(BuildingHub buildingHub)
    //{
    //    HubBuilding = buildingHub;
    //    thisBuildingName = HubBuilding.buildingName;
    //    transform.GetChild(0).GetComponent<Text>().text = thisBuildingName;
    //    buildingID = buildingHub.BuildingID;
    //}

    public void Die()
    {
        Destroy(gameObject);
    }
}
