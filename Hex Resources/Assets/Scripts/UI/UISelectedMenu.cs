using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISelectedMenu : MonoBehaviour
{
    public Text selectedName;
    public HexGameUI gameUI;
    public RectTransform listContent;
    public UIBuildingsIconItem buildingIconPrefab;
    public UIUnitsMenu unitsMenu;
    public UIBuildingsMenu buildingsMenu;
    public bool placeBuilding;
    public BuildingsRoot buildingsRoot;

    public UnitBuilder activeBuilder;

    public UnitsRoot unitsRoot;

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
        if (gameUI.selectedUnit.GetComponent<UnitBuilder>())
        {
            for (int i = 0; i < listContent.childCount; i++)
            {
                Destroy(listContent.GetChild(i).gameObject);
            }

            for (int i = 0; i < buildingsMenu.buildingsCollection.prefabs.Length; i++)
            {
                UIBuildingsIconItem buildingIcon = Instantiate(buildingIconPrefab);
                buildingIcon.selectedMenu = this;
                buildingIcon.buildingsMenu = null;
                buildingIcon.SetFields(buildingsMenu.buildingsCollection.PickBuilding(i));
                buildingIcon.transform.SetParent(listContent, false);
            }
        }
    }

    private void InitializeBuildingMenu()
    {
        SetTitle(gameUI.selectedBuilding.buildingName);
    }

    private void SetTitle(string title)
    {
        selectedName.text = title;
    }

    public IEnumerator PlaceBuilding(BuildingBaseClass building)
    {
        placeBuilding = true;
        activeBuilder = gameUI.selectedUnit.GetComponent<UnitBuilder>();
        BuildingBaseClass buildingLocal = Instantiate(building);
        Color c1, c2, c3;

        Material[] materialsArr = buildingLocal.GetComponentInChildren<MeshRenderer>().materials;

        Color validBuildColor = new Color (0.75f, 0.75f, 0.75f);
        Color invalidBuildColor = new Color (0.75f, 0.0f, 0.0f);

        c1 = materialsArr[0].color;
        c2 = materialsArr[1].color;
        c3 = materialsArr[2].color;
                
        while(placeBuilding)
        {
            if (gameUI.UpdateCurrentCell())
            {
                buildingLocal.transform.localPosition = gameUI.currentCell.Position;
                if (gameUI.currentCell.IsValidBuildLocation)
                {
                    for (int i = 0; i < materialsArr.Length; i++)
                    {
                        materialsArr[i].color = validBuildColor;
                    }
                }
                else if (!gameUI.currentCell.IsValidBuildLocation)
                {
                    for (int i = 0; i < materialsArr.Length; i++)
                    {
                        materialsArr[i].color = invalidBuildColor;
                    }
                }
                else
                {
                    Debug.Log("error with building material assignment");
                }
                yield return null;
            }
            
            if (Input.GetMouseButtonDown(0) && gameUI.currentCell.IsValidBuildLocation)
            {
                buildingLocal.transform.localScale = new Vector3 (0.0f, 0.0f, 0.0f);

                materialsArr[0].color = c1;
                materialsArr[1].color = c2;
                materialsArr[2].color = c3;
                yield return null;

                buildingLocal.GetComponentInChildren<MeshRenderer>().materials = materialsArr;
                yield return null;

                activeBuilder.BuildAtLocation(buildingLocal, gameUI.currentCell);

                gameUI.selectedUnit = null;

                placeBuilding = false;
                yield return null;
            }
            yield return null;
        }
        yield return null;
    }
}
