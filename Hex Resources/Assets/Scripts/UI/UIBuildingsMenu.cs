using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuildingsMenu : MonoBehaviour
{
    public RectTransform listContent;

    public UIBuildingsIconItem buildingIconPrefab;

    public HexFeatureCollection buildingsCollection;

    public BuildingsRoot buildingsRoot;

    public UIUnitsMenu unitsMenu;

    public void Open()
    {
        gameObject.SetActive(true);
        FillList();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SelectMenuItem(BuildingBaseClass building)
    {
        buildingsRoot.CreateBuilding(building, unitsMenu.unitsCollection.PickUnit(4));
        Close();
    }

    public void FillList()
    {
        for (int i = 0; i < listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < buildingsCollection.prefabs.Length; i++)
        {
            UIBuildingsIconItem menuIcon = Instantiate(buildingIconPrefab);
            menuIcon.buildingsMenu = this;
            menuIcon.SetFields(buildingsCollection.PickBuilding(i));
            menuIcon.transform.SetParent(listContent, false);
        }
    }
}
