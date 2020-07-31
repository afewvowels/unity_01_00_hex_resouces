using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexGameUI : MonoBehaviour
{
	public HexGrid grid;

	public HexCell currentCell;

	public List<HexCell> cellsWithHighlights = new List<HexCell>();

	[SerializeField]
	public static HexUnit selectedUnit;

	[SerializeField]
	public static BuildingBaseClass selectedBuilding;

	public static BuildingBaseClass buildingToBuild;

	//public UISelectedMenu selectedMenu;

	public UISecretMenu secretMenu;

	public static bool placeBuilding;

	public Text selectedTitle;

	public RectTransform listContent;

	public Canvas selectedCanvas;

	public BuildingsRoot buildingsRoot;

	public UnitsRoot unitsRoot;

	public ResourcesRoot resourcesRoot;

	public UIBuildingsIconItem buildingIconPrefab;
	public UIUnitsIconItem unitIconPrefab;
	public UISelectedIconItem selectedMenuIconPrefab;

	public UIGarageMenu garageMenu;
	public UIProductsMenu productsMenu;

	public HexCell cellWithHighlight;

	public GameObject duplicateBuildingMenu;

	public static bool menuOpen = true;

	private void FixedUpdate()
	{
		try
		{
			if (EventSystem.current.IsPointerOverGameObject() && !IsPointerOverUIElement())
			{
				if (Input.GetMouseButtonDown(0) && !placeBuilding && !menuOpen)
				{
					DoSelection();
				}
				else if (placeBuilding)
				{
					grid.ClearPath();
				}
				else if (selectedUnit)
				{
					if (Input.GetMouseButtonDown(1))
					{
						DoMove();
					}
					else
					{
						DoPathfinding();
					}
				}
			}
			else if (!EventSystem.current.IsPointerOverGameObject() && !IsPointerOverUIElement())
			{
				CloseSelectedMenu();
				ClearSelected();
			}
		}
		catch
		{
			Debug.Log("error doing hexgameui selection");
		}
		//else if (!EventSystem.current.IsPointerOverGameObject())
		//{
		//    ClearSelected();
		//}
		//else if (Input.GetKeyDown(KeyCode.P))
		//{
		//    if (!secretMenu.isOpen)
		//    {
		//        secretMenu.Close();
		//    }
		//    else
		//    {
		//        secretMenu.Open();
		//    }
		//}
	}

	public static void ClearSelected()
	{
		selectedUnit = null;
		selectedBuilding = null;
		placeBuilding = false;

		try
		{
			Destroy(buildingToBuild.gameObject);
			Debug.Log("Destroyed building");
		}
		catch
		{
			buildingToBuild = null;
		}
	}

	private void OpenSelectedMenu()
	{
		selectedCanvas.gameObject.SetActive(true);
		InitializeSelectedMenu();
	}

	private void CloseSelectedMenu()
	{
		selectedCanvas.gameObject.SetActive(false);
		ClearGridLayout();
	}

	public void OpenDuplicateBuildingWarningMenu()
	{
		HexGameUI.menuOpen = true;
		duplicateBuildingMenu.SetActive(true);
	}

	public void CloseDuplicateBuildingWarningMenu()
	{
		HexGameUI.menuOpen = false;
		duplicateBuildingMenu.SetActive(false);
	}

	private void ClearGridLayout()
	{
		for (int i = 0; i < listContent.childCount; i++)
		{
			if (listContent.GetChild(i).GetComponent<UISelectedIconItem>())
			{
				listContent.GetChild(i).GetComponent<UISelectedIconItem>().Die();
			}
			else if (listContent.GetChild(i).GetComponent<UIBuildingsIconItem>())
			{
				listContent.GetChild(i).GetComponent<UIBuildingsIconItem>().Die();
			}
		}
	}

	public void SetEditMode(bool toggle)
	{
		enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
		if (toggle)
		{
			Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
		}
		else
		{
			Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
		}
	}

	public bool UpdateCurrentCell()
	{
		HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell)
		{
			currentCell = cell;
			return true;
		}
		return false;
	}

	private void DoSelection()
	{
		TurnOffNeighboringHighlights();
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell.Unit && !currentCell.Unit.IsBusy)
		{
			selectedUnit = currentCell.Unit;
			selectedBuilding = null;
			OpenSelectedMenu();
			currentCell.EnableHighlight(Color.white);
			cellsWithHighlights.Add(currentCell);
			cellWithHighlight = currentCell;
		}
		else if (currentCell.Building && !currentCell.Building.IsBusy)
		{
			selectedBuilding = currentCell.Building;
			selectedUnit = null;
			if (selectedBuilding.GetComponent<BuildingGarage>())
			{
				selectedBuilding.GetComponent<BuildingGarage>().garageMenu.Open();
			}
			else if (selectedBuilding.GetComponent<BuildingFactory>())
			{
				selectedBuilding.GetComponent<BuildingFactory>().productsMenu.Open();
			}
			else
			{
				OpenSelectedMenu();
			}
			cellWithHighlight = currentCell;
			cellWithHighlight.EnableHighlight(Color.white);
			cellsWithHighlights.Add(cellWithHighlight);
		}
		else
		{
			CloseSelectedMenu();
			cellWithHighlight.DisableHighlight();
			selectedUnit = null;
			selectedBuilding = null;
		}
	}

	private void DoPathfinding()
	{
		if (UpdateCurrentCell() && selectedUnit)
		{
			if (currentCell && selectedUnit.IsValidDestination(currentCell) && currentCell.Explorable && !selectedUnit.IsBusy && grid.Search(selectedUnit.Location, currentCell))
			{
				grid.FindPath(selectedUnit.Location, currentCell, 24, false);
				TurnOffNeighboringHighlights();
				currentCell.EnableHighlight(Color.green);
				selectedUnit.Location.EnableHighlight(Color.white);
				cellsWithHighlights.Add(currentCell);
				cellsWithHighlights.Add(selectedUnit.Location);
			}
			else if (selectedUnit.IsBusy)
			{
				TurnOffNeighboringHighlights();
			}
			else
			{
				TurnOffNeighboringHighlights();
				currentCell.EnableHighlight(Color.red);
				selectedUnit.Location.EnableHighlight(Color.white);
				cellsWithHighlights.Add(currentCell);
				cellsWithHighlights.Add(selectedUnit.Location);
				grid.ClearPath();
			}
		}
	}

	private void DoMove()
	{
		if (grid.HasPath)
		{
			selectedUnit.Travel(grid.GetPath());
			grid.ClearPath();
		}
	}

	private void DoCancel()
	{
		Debug.Log("Cancel action");
	}

	private void DoDie()
	{
		selectedUnit.Die();
		selectedUnit = null;
		CloseSelectedMenu();
	}

	private void DoBuild()
	{
		ClearGridLayout();
		InitializeBuildingMenu();
	}

	private void DoHarvest()
	{
		selectedUnit.GetComponent<UnitHarvester>().needsDestination = true;
	}

	private void DoBuildingAddOn()
	{
		ClearGridLayout();
		InitializeHubAddOnMenu();
	}

	private void DoPlaceBeacon()
	{
		TurnOffNeighboringHighlights();
		try
		{
			StartCoroutine(PlaceResourceBuilding(selectedUnit.GetComponent<UnitSurveyor>().beaconPrefab));
		}
		catch
		{
			Debug.Log("error initializing place beacon action chain");
		}
	}

	private void DoPlaceResonator()
	{
		TurnOffNeighboringHighlights();
		try
		{
			StartCoroutine(PlaceResourceBuilding(selectedUnit.GetComponent<UnitSurveyor>().resonatorPrefab));
		}
		catch
		{
			Debug.Log("error initializing place resonator action chain");
		}
	}

	public void DoSelectedAction(UISelectedIconItem actionItem)
	{
		switch (actionItem.actionName)
		{
			case "Move":
				DoMove();
				break;
			case "Cancel":
				DoCancel();
				break;
			case "Die":
				DoDie();
				break;
			case "Build":
				DoBuild();
				break;
			case "Harvest":
				DoHarvest();
				break;
			case "Add On":
				DoBuildingAddOn();
				break;
			case "Beacon":
				DoPlaceBeacon();
				break;
			case "Resonator":
				DoPlaceResonator();
				break;
			default:
				break;
		}
	}

	public static bool IsPointerOverUIElement()
	{
		return IsPointerOverUIElement(GetEventSystemRaycastResults());
	}

	public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
	{
		for (int index = 0; index < eventSystemRaysastResults.Count; index++)
		{
			RaycastResult curRaysastResult = eventSystemRaysastResults[index];
			if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("selectedui"))
				return true;
		}
		return false;
	}

	static List<RaycastResult> GetEventSystemRaycastResults()
	{
		PointerEventData eventData = new PointerEventData(EventSystem.current);
		eventData.position = Input.mousePosition;
		List<RaycastResult> raysastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, raysastResults);
		return raysastResults;
	}

	private void ClearListContent()
	{
		for (int i = 0; i < listContent.childCount; i++)
		{
			Destroy(listContent.GetChild(i));
		}
	}

	private void InitializeSelectedMenu()
	{
		if (selectedUnit)
		{
			InitializeUnitActionsMenu();
		}
		else if (selectedBuilding)
		{
			InitializeBuildingsActionMenu();
		}
	}

	private void InitializeUnitActionsMenu()
	{
		if (listContent.childCount == 0)
		{
			SetTitle(selectedUnit.unitName);
			InitializeUnitActionMove();
			InitializeUnitActionCancel();
			InitializeUnitActionDie();
			if (selectedUnit.GetComponent<UnitBuilder>())
			{
				InitializeUnitActionBuild();
			}
			if (selectedUnit.GetComponent<UnitHarvester>())
			{
				InitializeUnitActionHarvest();
			}
			if (selectedUnit.GetComponent<UnitSurveyor>())
			{
				InitializeUnitActionBeacon();
				if (BuildingHarmonicResonator.isAvailable)
				{
					InitializeUnitActionHarmonizer();
				}
			}
		}
	}

	private void InitializeBuildingsActionMenu()
	{
		SetTitle(selectedBuilding.buildingName);
		ClearGridLayout();

		if (selectedBuilding.GetComponent<BuildingHub>())
		{
			InitializeHubAddOnMenu();
		}
	}

	private void InitializeUnitMenu()
	{
		SetTitle(selectedUnit.unitName);

		ClearListContent();

		if (selectedUnit.GetComponent<UnitBuilder>())
		{
			for (int i = 0; i < buildingsRoot.buildingsCollection.prefabs.Length; i++)
			{
				UIBuildingsIconItem buildingIcon = Instantiate(buildingIconPrefab);
				buildingIcon.hexGameUI = this;
				buildingIcon.buildingsMenu = null;
				buildingIcon.SetFields(buildingsRoot.buildingsCollection.PickBuilding(i));
				buildingIcon.transform.SetParent(listContent, false);
			}
		}
	}

	private void InitializeBuildingMenu()
	{
		SetTitle("Build");

		ClearListContent();

		for (int i = 0; i < buildingsRoot.buildingsCollection.prefabs.Length; i++)
		{
			UIBuildingsIconItem buildingIcon = Instantiate(buildingIconPrefab);
			buildingIcon.hexGameUI = this;
			buildingIcon.buildingsMenu = null;
			buildingIcon.SetFields(buildingsRoot.buildingsCollection.PickBuilding(i));
			buildingIcon.transform.SetParent(listContent, false);
		}
	}

	private void InitializeHubAddOnMenu()
	{
		SetTitle("Add Ons");

		ClearListContent();

		for (int i = 0; i < selectedBuilding.GetComponent<BuildingHub>().hubAddOns.Count; i++)
		{
			if (selectedBuilding.GetComponent<BuildingHub>().hubAddOns[i].IsAvailable)
			{
				UIBuildingsIconItem buildingIcon = Instantiate(buildingIconPrefab);
				buildingIcon.hexGameUI = this;
				buildingIcon.buildingsMenu = null;
				buildingIcon.HubBuilding = selectedBuilding.GetComponent<BuildingHub>();
				buildingIcon.SetFields(selectedBuilding.GetComponent<BuildingHub>().hubAddOns[i]);
				buildingIcon.transform.SetParent(listContent, false);
			}
		}
	}

	private void InitializeUnitActionMove()
	{
		UISelectedIconItem unitAction = Instantiate(selectedMenuIconPrefab);
		unitAction.hexGameUI = this;
		unitAction.SetFields("Move");
		unitAction.transform.SetParent(listContent, false);
	}

	private void InitializeUnitActionCancel()
	{
		UISelectedIconItem unitAction = Instantiate(selectedMenuIconPrefab);
		unitAction.hexGameUI = this;
		unitAction.SetFields("Cancel");
		unitAction.transform.SetParent(listContent, false);
	}

	private void InitializeUnitActionDie()
	{
		UISelectedIconItem unitAction = Instantiate(selectedMenuIconPrefab);
		unitAction.hexGameUI = this;
		unitAction.SetFields("Die");
		unitAction.transform.SetParent(listContent, false);
	}

	private void InitializeUnitActionBuild()
	{
		UISelectedIconItem unitAction = Instantiate(selectedMenuIconPrefab);
		unitAction.hexGameUI = this;
		unitAction.SetFields("Build");
		unitAction.transform.SetParent(listContent, false);
	}

	private void InitializeUnitActionHarvest()
	{
		UISelectedIconItem unitAction = Instantiate(selectedMenuIconPrefab);
		unitAction.hexGameUI = this;
		unitAction.SetFields("Harvest");
		unitAction.transform.SetParent(listContent, false);
	}

	private void InitializeUnitActionBeacon()
	{
		UISelectedIconItem unitAction = Instantiate(selectedMenuIconPrefab);
		unitAction.hexGameUI = this;
		unitAction.SetFields("Beacon");
		unitAction.transform.SetParent(listContent, false);
	}

	private void InitializeUnitActionHarmonizer()
	{
		UISelectedIconItem unitAction = Instantiate(selectedMenuIconPrefab);
		unitAction.hexGameUI = this;
		unitAction.SetFields("Resonator");
		unitAction.transform.SetParent(listContent, false);
	}

	private void InitializeBuildingActionAddOn()
	{
		UISelectedIconItem buildingAction = Instantiate(selectedMenuIconPrefab);
		buildingAction.hexGameUI = this;
		buildingAction.SetFields("Add On");
		buildingAction.transform.SetParent(listContent, false);
	}

	private void SetTitle(string title)
	{
		selectedTitle.text = title;
	}

	public IEnumerator PlaceBuilding(BuildingBaseClass building)
	{
		placeBuilding = true;
		UnitBuilder activeBuilder = selectedUnit.GetComponent<UnitBuilder>();
		BuildingBaseClass buildingLocal = Instantiate(building);
		Color c1, c2, c3;

		Material[] materialsArr = buildingLocal.GetComponentInChildren<MeshRenderer>().materials;

		Color validBuildColor = new Color(0.75f, 0.75f, 0.75f);
		Color invalidBuildColor = new Color(0.75f, 0.0f, 0.0f);

		c1 = materialsArr[0].color;
		c2 = materialsArr[1].color;
		c3 = materialsArr[2].color;

		while (placeBuilding)
		{
			if (UpdateCurrentCell())
			{
				buildingLocal.transform.localPosition = currentCell.Position;
				if (currentCell.IsValidBuildLocation)
				{
					for (int i = 0; i < materialsArr.Length; i++)
					{
						materialsArr[i].color = validBuildColor;
					}
				}
				else if (!currentCell.IsValidBuildLocation)
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

			if (Input.GetMouseButtonDown(0) && currentCell.IsValidBuildLocation)
			{
				buildingLocal.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);

				materialsArr[0].color = c1;
				materialsArr[1].color = c2;
				materialsArr[2].color = c3;
				yield return null;

				buildingLocal.GetComponentInChildren<MeshRenderer>().materials = materialsArr;
				yield return null;

				activeBuilder.BuildAtLocation(buildingLocal, currentCell);

				selectedUnit = null;

				CloseSelectedMenu();

				placeBuilding = false;
				yield return null;
			}

			yield return null;
		}

		yield return null;
	}

	public IEnumerator PlaceResourceBuilding(BuildingBaseClass building)
	{
		placeBuilding = true;
		BuildingBaseClass buildingLocal = Instantiate(building);
		buildingToBuild = buildingLocal;

		buildingLocal.Grid = grid;

		Material[] materialsArr = buildingLocal.GetComponentInChildren<MeshRenderer>().materials;
		Color[] colorsArr = new Color[materialsArr.Length];

		Color validBuildColor = new Color(0.75f, 0.75f, 0.75f);
		Color invalidBuildColor = new Color(0.75f, 0.0f, 0.0f);

		for (int i = 0; i < materialsArr.Length; i++)
		{
			colorsArr[i] = materialsArr[i].color;
		}

		while (placeBuilding)
		{
			if (UpdateCurrentCell())
			{
				//TurnOffNeighboringHighlights(currentCell);
				TurnOffNeighboringHighlights();
				buildingLocal.transform.position = currentCell.Position;
				if (currentCell.HasResource && currentCell.IsExplored)
				{
					if (buildingLocal.GetComponent<BuildingHarmonicResonator>() && currentCell.resonator)
					{
						currentCell.EnableHighlight(Color.red);
						cellsWithHighlights.Add(currentCell);
						for (int i = 0; i < materialsArr.Length; i++)
						{
							materialsArr[i].color = invalidBuildColor;
						}
					}
					else if (buildingLocal.GetComponent<BuildingSurveyBeacon>() && currentCell.beacon)
					{
						currentCell.EnableHighlight(Color.red);
						cellsWithHighlights.Add(currentCell);
						for (int i = 0; i < materialsArr.Length; i++)
						{
							materialsArr[i].color = invalidBuildColor;
						}
					}
					else
					{
						currentCell.EnableHighlight(Color.green);
						cellsWithHighlights.Add(currentCell);
						for (int i = 0; i < materialsArr.Length; i++)
						{
							materialsArr[i].color = validBuildColor;
						}
					}
				}
				else
				{
					currentCell.EnableHighlight(Color.red);
					cellsWithHighlights.Add(currentCell);
					for (int i = 0; i < materialsArr.Length; i++)
					{
						materialsArr[i].color = invalidBuildColor;
					}
				}
				yield return null;
			}

			if (Input.GetMouseButtonDown(0) && currentCell.HasResource)
			{
				if (buildingLocal.GetComponent<BuildingHarmonicResonator>() && currentCell.resonator)
				{
					OpenDuplicateBuildingWarningMenu();
				}
				else if (buildingLocal.GetComponent<BuildingSurveyBeacon>() && currentCell.beacon)
				{
					OpenDuplicateBuildingWarningMenu();
				}
				else
				{
					buildingLocal.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);

					for (int i = 0; i < materialsArr.Length; i++)
					{
						materialsArr[i].color = colorsArr[i];
					}

					selectedUnit.GetComponent<UnitSurveyor>().BuildAtResource(buildingLocal, currentCell);

					placeBuilding = false;
					buildingToBuild = null;
				}
			}
			else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
			{
				Destroy(buildingLocal.gameObject);
				placeBuilding = false;
				buildingToBuild = null;
			}
			yield return null;
		}
		//TurnOffNeighboringHighlights(currentCell);
		TurnOffNeighboringHighlights();

		//try
		//{
		//    Destroy(buildingLocal.gameObject);
		//}
		//catch
		//{
		//    Debug.Log("no building to destroy");
		//}

		yield return null;
	}

	public IEnumerator PlaceHubBuilding(BuildingBaseClass building)
	{
		placeBuilding = true;
		UnitBuilder activeBuilder = selectedUnit.GetComponent<UnitBuilder>();
		BuildingBaseClass buildingLocal = Instantiate(building);
		buildingToBuild = buildingLocal;

		buildingLocal.Grid = grid;

		Material[] materialsArr = buildingLocal.GetComponentInChildren<MeshRenderer>().materials;
		Color[] colorsArr = new Color[materialsArr.Length];

		Color validBuildColor = new Color(0.75f, 0.75f, 0.75f);
		Color invalidBuildColor = new Color(0.75f, 0.0f, 0.0f);

		for (int i = 0; i < materialsArr.Length; i++)
		{
			colorsArr[i] = materialsArr[i].color;
		}

		while (placeBuilding)
		{
			if (UpdateCurrentCell())
			{
				//TurnOffNeighboringHighlights(currentCell);
				TurnOffNeighboringHighlights();
				TurnOnNeighboringHighlights(currentCell);
				buildingLocal.transform.position = currentCell.Position;
				if (ValidateNeighboringCellsBuild(currentCell))
				{
					for (int i = 0; i < materialsArr.Length; i++)
					{
						materialsArr[i].color = validBuildColor;
					}
				}
				else
				{
					for (int i = 0; i < materialsArr.Length; i++)
					{
						materialsArr[i].color = invalidBuildColor;
					}
				}
				yield return null;
			}

			if (Input.GetMouseButtonDown(0) && ValidateNeighboringCellsBuild(currentCell))
			{

				buildingLocal.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
				//TurnOffNeighboringHighlights(currentCell);
				TurnOffNeighboringHighlights();

				for (int i = 0; i < materialsArr.Length; i++)
				{
					materialsArr[i].color = colorsArr[i];
					yield return null;
				}

				buildingLocal.GetComponentInChildren<MeshRenderer>().materials = materialsArr;

				activeBuilder.BuildAtLocation(buildingLocal, currentCell);

				foreach (HexCell neighbor in currentCell.GetNeighbors())
				{
					neighbor.Elevation = currentCell.Elevation;
				}

				selectedUnit = null;
				CloseSelectedMenu();
				placeBuilding = false;
				buildingToBuild = null;
				yield return null;
			}

			if (Input.GetMouseButtonDown(1))
			{
				//TurnOffNeighboringHighlights(currentCell);
				TurnOffNeighboringHighlights();
				Destroy(buildingLocal.gameObject);
				CloseSelectedMenu();
				selectedUnit = null;
				placeBuilding = false;
				buildingToBuild = null;
			}
			yield return null;
		}

		//try
		//{
		//    TurnOffNeighboringHighlights();
		//    Destroy(buildingLocal.gameObject);
		//}
		//catch
		//{
		//    Debug.Log("no building to destroy");
		//}

		yield return null;
	}

	public IEnumerator PlaceHubAddOnBuilding(BuildingHub hubBuilding, BuildingBaseClass building)
	{
		HexCell depositCell = null;
		placeBuilding = true;
		BuildingBaseClass buildingLocal = Instantiate(building);
		buildingLocal.Grid = grid;
		buildingToBuild = buildingLocal;
		List<Material> materialsArr = new List<Material>();

		bool isValidLocation = false;

		for (int i = 0; i < buildingLocal.GetComponentsInChildren<MeshRenderer>().Length; i++)
		{
			foreach (Material material in buildingLocal.GetComponentsInChildren<MeshRenderer>()[i].materials)
			{
				materialsArr.Add(material);
			}
		}
		Color[] colorsArr = new Color[materialsArr.Count];


		Color validBuildColor = new Color(0.75f, 0.75f, 0.75f);
		Color invalidBuildColor = new Color(0.75f, 0.0f, 0.0f);

		for (int i = 0; i < materialsArr.Count; i++)
		{
			colorsArr[i] = materialsArr[i].color;
		}

		for (int i = 0; i < colorsArr.Length; i++)
		{
			materialsArr[i].color = validBuildColor;
		}

		while (placeBuilding)
		{
			if (UpdateCurrentCell())
			{
				float addonOrientation = 0.0f;

				//TurnOffNeighboringHighlights(hubBuilding.Location);
				TurnOffNeighboringHighlights();
				TurnOnNeighboringHighlights(hubBuilding.Location);

				if (currentCell.IsNeighbor(hubBuilding.Location))
				{

					for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
					{
						if (hubBuilding.Location.GetNeighbor(d) == currentCell)
						{
							switch (d)
							{
								case HexDirection.NE:
									depositCell = currentCell.GetNeighbor(d);
									if (buildingLocal.GetComponent<BuildingProcessing>())
									{
										//depositCell = currentCell.GetNeighbor(d);
										buildingLocal.GetComponent<BuildingProcessing>().dropoffCell = depositCell;
									}
									if (buildingLocal.GetComponent<BuildingGarage>())
									{
										//depositCell = currentCell.GetNeighbor(d);
										buildingLocal.GetComponent<BuildingGarage>().buildDestination = depositCell;
									}
									addonOrientation = 30.0f;
									break;
								case HexDirection.E:
									depositCell = currentCell.GetNeighbor(d);
									if (buildingLocal.GetComponent<BuildingProcessing>())
									{
										//depositCell = currentCell.GetNeighbor(d);
										buildingLocal.GetComponent<BuildingProcessing>().dropoffCell = depositCell;
									}
									if (buildingLocal.GetComponent<BuildingGarage>())
									{
										//depositCell = currentCell.GetNeighbor(d);
										buildingLocal.GetComponent<BuildingGarage>().buildDestination = depositCell;
									}
									addonOrientation = 90.0f;
									break;
								case HexDirection.SE:
									depositCell = currentCell.GetNeighbor(d);
									if (buildingLocal.GetComponent<BuildingProcessing>())
									{
										//depositCell = currentCell.GetNeighbor(d);
										buildingLocal.GetComponent<BuildingProcessing>().dropoffCell = depositCell;
									}
									if (buildingLocal.GetComponent<BuildingGarage>())
									{
										//depositCell = currentCell.GetNeighbor(d);
										buildingLocal.GetComponent<BuildingGarage>().buildDestination = depositCell;
									}
									addonOrientation = 150.0f;
									break;
								case HexDirection.SW:
									depositCell = currentCell.GetNeighbor(d);
									if (buildingLocal.GetComponent<BuildingProcessing>())
									{
										//depositCell = currentCell.GetNeighbor(d);
										buildingLocal.GetComponent<BuildingProcessing>().dropoffCell = depositCell;
									}
									if (buildingLocal.GetComponent<BuildingGarage>())
									{
										//depositCell = currentCell.GetNeighbor(d);
										buildingLocal.GetComponent<BuildingGarage>().buildDestination = depositCell;
									}
									addonOrientation = 210.0f;
									break;
								case HexDirection.W:
									depositCell = currentCell.GetNeighbor(d);
									if (buildingLocal.GetComponent<BuildingProcessing>())
									{
										//depositCell = currentCell.GetNeighbor(d);
										buildingLocal.GetComponent<BuildingProcessing>().dropoffCell = depositCell;
									}
									if (buildingLocal.GetComponent<BuildingGarage>())
									{
										//depositCell = currentCell.GetNeighbor(d);
										buildingLocal.GetComponent<BuildingGarage>().buildDestination = depositCell;
									}
									addonOrientation = 270.0f;
									break;
								case HexDirection.NW:
									depositCell = currentCell.GetNeighbor(d);
									if (buildingLocal.GetComponent<BuildingProcessing>())
									{
										//depositCell = currentCell.GetNeighbor(d);
										buildingLocal.GetComponent<BuildingProcessing>().dropoffCell = depositCell;
									}
									if (buildingLocal.GetComponent<BuildingGarage>())
									{
										//depositCell = currentCell.GetNeighbor(d);
										buildingLocal.GetComponent<BuildingGarage>().buildDestination = depositCell;
									}
									addonOrientation = 330.0f;
									break;
								default:
									break;
							}
						}
					}

					if (depositCell)
					{
						if (Mathf.Abs(currentCell.Elevation - depositCell.Elevation) < 2)
						{
							isValidLocation = currentCell.IsValidBuildLocation && depositCell.IsValidBuildLocation;
						}
					}
					else
					{
						isValidLocation = currentCell.IsValidBuildLocation;
					}



					if (currentCell.IsValidBuildLocation && buildingLocal.GetComponent<BuildingFactory>())
					{
						currentCell.EnableHighlight(Color.green);
						cellsWithHighlights.Add(currentCell);
						for (int i = 0; i < materialsArr.Count; i++)
						{
							materialsArr[i].color = validBuildColor;
						}
						isValidLocation = true;
					}
					else if (currentCell.IsValidBuildLocation && depositCell.IsValidBuildLocation && isValidLocation)
					{
						currentCell.EnableHighlight(Color.green);
						depositCell.EnableHighlight(Color.green);
						cellsWithHighlights.Add(currentCell);
						cellsWithHighlights.Add(depositCell);
						for (int i = 0; i < materialsArr.Count; i++)
						{
							materialsArr[i].color = validBuildColor;
						}

						isValidLocation = true;
					}
					else if (currentCell.IsValidBuildLocation && !depositCell.IsValidBuildLocation || !isValidLocation)
					{
						currentCell.EnableHighlight(Color.green);
						depositCell.EnableHighlight(Color.red);
						cellsWithHighlights.Add(currentCell);
						cellsWithHighlights.Add(depositCell);
						for (int i = 0; i < materialsArr.Count; i++)
						{
							materialsArr[i].color = invalidBuildColor;
						}

						isValidLocation = false;
					}
					else if (!currentCell.IsValidBuildLocation)
					{
						currentCell.EnableHighlight(Color.red);
						cellsWithHighlights.Add(currentCell);
						for (int i = 0; i < materialsArr.Count; i++)
						{
							materialsArr[i].color = invalidBuildColor;
						}

						isValidLocation = false;
					}

					buildingLocal.transform.position = currentCell.Position;

					buildingLocal.Orientation = addonOrientation;
					yield return null;
				}
				yield return null;
			}

			if (Input.GetMouseButtonDown(0) && hubBuilding.Location.IsNeighbor(currentCell) && isValidLocation)
			{
				Vector3 initialScale = new Vector3(0.0f, 0.0f, 0.0f);
				Vector3 endScale = new Vector3(1.0f, 1.0f, 1.0f);

				if (buildingLocal.GetComponent<BuildingGarage>())
				{
					buildingLocal.GetComponent<BuildingGarage>().garageMenu = garageMenu;

					selectedBuilding.GetComponent<BuildingHub>().RemoveGarageFromList();
				}
				else if (buildingLocal.GetComponent<BuildingFactory>())
				{
					buildingLocal.GetComponent<BuildingFactory>().productsMenu = productsMenu;

					selectedBuilding.GetComponent<BuildingHub>().RemoveFactoryFromList();
				}
				else if (buildingLocal.GetComponent<BuildingProcessing>())
				{
					selectedBuilding.GetComponent<BuildingHub>().RemoveProcessingFromList();
				}

				//TurnOffNeighboringHighlights(hubBuilding.Location);
				TurnOffNeighboringHighlights();

				//for (int i = 0; i < colorsArr.Length; i++)
				//{
				//    materialsArr[i].color = colorsArr[i];
				//}

				for (float t = 0.0f; t <= 1.0f; t += Time.deltaTime)
				{
					buildingLocal.transform.localScale = Vector3.Lerp(initialScale, endScale, t);
					buildingLocal.transform.position = Vector3.Lerp(hubBuilding.Location.Position, currentCell.Position, t);
					yield return null;
				}

				buildingLocal.Location = currentCell;
				buildingLocal.StartBuildMe(colorsArr);

				CloseSelectedMenu();
				placeBuilding = false;
				buildingToBuild = null;

			}

			if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
			{
				//TurnOffNeighboringHighlights(hubBuilding.Location);
				TurnOffNeighboringHighlights();
				//buildingLocal.Die();
				Destroy(buildingLocal.gameObject);

				CloseSelectedMenu();
				placeBuilding = false;
				buildingToBuild = null;
				yield return null;
			}
			yield return null;
		}
		yield return null;

		//try
		//{
		//    TurnOffNeighboringHighlights();
		//    Destroy(buildingLocal.gameObject);
		//}
		//catch
		//{
		//    Debug.Log("no building to destroy");
		//}
	}

	private bool ValidateNeighboringCellsBuild(HexCell cell)
	{
		bool isValid = true;

		if (!cell.IsValidBuildLocation)
		{
			return false;
		}

		foreach (HexCell neighbor in cell.GetNeighbors())
		{
			if (!neighbor.IsValidBuildLocation)
			{
				isValid = false;
			}
		}

		return isValid;
	}

	private bool ValidateNeighboringCellBuild(HexCell cell)
	{
		if (!cell.IsValidBuildLocation)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	private void TurnOnNeighboringHighlights(HexCell cell)
	{
		if (cellsWithHighlights.Count > 0)
		{
			TurnOffNeighboringHighlights();
		}

		if (cell)
		{
			foreach (HexCell neighbor in cell.GetNeighbors())
			{
				if (neighbor.IsValidBuildLocation)
				{
					neighbor.EnableHighlight(Color.white);
					cellsWithHighlights.Add(neighbor);
				}
				else
				{
					neighbor.EnableHighlight(Color.red);
					cellsWithHighlights.Add(neighbor);
				}
			}

			if (cell.IsValidBuildLocation)
			{
				cell.EnableHighlight(Color.white);
				cellsWithHighlights.Add(cell);
			}
			else
			{
				cell.EnableHighlight(Color.red);
				cellsWithHighlights.Add(cell);
			}
		}
	}

	private void TurnOffNeighboringHighlights(HexCell cell)
	{
		foreach (HexCell neighbor in cell.GetNeighbors())
		{
			foreach (HexCell extraNeighbor in neighbor.GetNeighbors())
			{
				extraNeighbor.DisableHighlight();
			}

			neighbor.DisableHighlight();
		}

		cell.DisableHighlight();
	}

	public void TurnOffNeighboringHighlights()
	{
		foreach (HexCell cell in cellsWithHighlights)
		{
			cell.DisableHighlight();
		}

		cellsWithHighlights.Clear();
	}
}
