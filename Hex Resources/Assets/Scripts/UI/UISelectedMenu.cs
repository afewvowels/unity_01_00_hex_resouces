using UnityEngine;
using UnityEngine.UI;

public class UISelectedMenu : MonoBehaviour
{
    public Text selectedName;
    public HexGameUI gameUI;

    public UIUnitsMenu unitsMenu;
    public UIBuildingsMenu buildingsMenu;

    public void Start()
    {
        gameUI = GetComponentInParent<HexGameUI>();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        if (gameUI.selectedUnit)
        {
            InitializeUnitMenu();
        }
        else if (gameUI.selectedBuilding)
        {
            InitializeBuildingMenu();
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void InitializeUnitMenu()
    {
        SetTitle(gameUI.selectedUnit.unitName);
    }

    private void InitializeBuildingMenu()
    {
        SetTitle(gameUI.selectedBuilding.buildingName);
    }

    private void SetTitle(string title)
    {
        selectedName.text = title;
    }
}
