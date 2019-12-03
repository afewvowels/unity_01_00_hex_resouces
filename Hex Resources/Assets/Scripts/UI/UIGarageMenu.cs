using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGarageMenu : MonoBehaviour
{
    public RectTransform availableUnitsList;
    public RectTransform queueList;

    public Image unitPreviewImage;

    public Text unitTitleText;
    public Text unitDescriptionText;
    public Text unitCostText;
    public Text unitDynamiCostText;

    public GameObject buildButton;

    public HexGameUI hexGameUI;

    public HexGrid grid;

    public BuildingGarage buildingGarage;

    public HexUnit unitToBuild;

    public UnitsRoot unitsRoot;

    public UIGarageUnitIconItem unitIconPrefab;

    private void Awake()
    {
        unitPreviewImage.gameObject.SetActive(false);
        unitTitleText.gameObject.SetActive(false);
        unitDescriptionText.gameObject.SetActive(false);
        unitCostText.gameObject.SetActive(false);
        unitDynamiCostText.gameObject.SetActive(false);
        buildButton.SetActive(false);

        buildingGarage = HexGameUI.selectedBuilding.GetComponent<BuildingGarage>();
    }

    public void Open()
    {
        HexGameUI.menuOpen = true;
        ClearAvailableUnitsList();
        InitializeUnitsToBuildView();
        InitializeUnitsQueue();
        HexMapCamera.Locked = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        hexGameUI.TurnOffNeighboringHighlights();
        HexGameUI.menuOpen = false;
        ClearAvailableUnitsList();
        HexMapCamera.Locked = false;
        gameObject.SetActive(false);
    }

    public void ClearAvailableUnitsList()
    {
        for (int i = 0; i < availableUnitsList.childCount; i++)
        {
            Destroy(availableUnitsList.GetChild(i).gameObject);
        }

        for (int i = 0; i < queueList.childCount; i++)
        {
            Destroy(queueList.GetChild(i).gameObject);
        }
    }

    public void InitializeUnitsToBuildView()
    {
        for (int i = 0; i < unitsRoot.unitsCollection.prefabs.Length; i++)
        {
            if (unitsRoot.unitsCollection.PickUnit(i).GetComponent<HexUnit>().IsUnlocked)
            {
                UIGarageUnitIconItem icon = Instantiate(unitIconPrefab);
                icon.InitializeGarageUnitIconButton(unitsRoot.unitsCollection.PickUnit(i));
                icon.garageMenu = this;
                icon.isInQueue = false;
                icon.transform.SetParent(availableUnitsList.transform, false);
            }
        }
    }

    public void InitializeUnitsQueue()
    {
        try
        {
            UIGarageUnitInProduction[] uips = buildingGarage.GetComponents<UIGarageUnitInProduction>();

            for (int i = 0; i < uips.Length; i++)
            {
                UIGarageUnitIconItem icon = Instantiate(unitIconPrefab);
                icon.isInQueue = true;
                icon.garageMenu = this;
                icon.uip = uips[i];
                icon.InitializeGarageUnitIconButton(uips[i].unitToBuild);
                icon.transform.SetParent(queueList.transform, false);
            }
        }
        catch
        {
            Debug.Log("no units in production");
        }
    }

    public void SelectUnitToBuild(HexUnit unit)
    {
        unitToBuild = unit;
        TurnOnAndInitializeUnitInfo();
    }

    private void TurnOnAndInitializeUnitInfo()
    {
        unitPreviewImage.gameObject.SetActive(true);
        unitTitleText.gameObject.SetActive(true);
        unitDescriptionText.gameObject.SetActive(true);
        unitCostText.gameObject.SetActive(true);
        unitDynamiCostText.gameObject.SetActive(true);
        buildButton.SetActive(true);

        //if (unitToBuild.GetComponent<UnitBuilder>())
        //{
        //    unitTitleText.text = UnitBuilder.builderName;
        //    unitDescriptionText.text = UnitBuilder.builderDescription;
        //    unitDynamiCostText.text = UnitBuilder.builderCost.ToString();
        //}
        //else if (unitToBuild.GetComponent<UnitHarvester>())
        //{
        //    unitTitleText.text = UnitHarvester.harvesterName;
        //    unitDescriptionText.text = UnitHarvester.harvesterDescription;
        //    unitDynamiCostText.text = UnitHarvester.harvesterCost.ToString();
        //}
        //else
        //{
        unitTitleText.text = unitToBuild.UnitName;
        unitDescriptionText.text = unitToBuild.UnitDescription;
        unitDynamiCostText.text = unitToBuild.UnitCost.ToString();
        //}
    }

    public void BuildUnit()
    {
        if (unitToBuild.UnitCost <= Economy.Dollars)
        {
            Economy.Dollars -= unitToBuild.UnitCost;
            UIGarageUnitInProduction uip = buildingGarage.gameObject.AddComponent<UIGarageUnitInProduction>();
            uip.unitToBuild = unitToBuild;
            uip.garageMenu = this;
            uip.buildingGarage = buildingGarage;
            uip.grid = grid;
            uip.initialized = true;
            ClearAvailableUnitsList();
            InitializeUnitsToBuildView();
            InitializeUnitsQueue();
            //buildingGarage.Location.DisableHighlight();
            //Close();
        }
    }

    //public void BuildUnitCoroutine()
    //{
    //    HexUnit unit = Instantiate(unitToBuild);
    //    grid.AddUnit(unit, buildingGarage.buildDestination, buildingGarage.orientation);
    //    unit.InitializeSpawn(buildingGarage.Location, buildingGarage.buildDestination);
    //}
}
