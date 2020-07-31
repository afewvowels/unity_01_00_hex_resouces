using UnityEngine;
using UnityEngine.UI;

public class UITechMenu : MonoBehaviour
{
	public Text upgradeTitle;
	public Text upgradeDescription;
	public Text upgradeCost;
	public GameObject upgradeButton;

	public BuildingsRoot buildingsRoot;
	public UnitsRoot unitsRoot;
	public ResourcesRoot resourcesRoot;

	public RectTransform listContent;
	public RectTransform queueListContent;

	public UITechCategoryButton techCategoryPrefab;
	public UITechObjectButton techObjectPrefab;
	public UITechUpgradeButton techUpgradePrefab;

	public string upgradeClassName;
	public TechTree.Item upgradeValues;

	public TechRoot techRoot;

	public GameObject errorPanel;

	private bool isOpen;

	public void Open()
	{
		if (!isOpen)
		{
			HexGameUI.menuOpen = true;
			gameObject.SetActive(true);
			TurnOffUpgradeItems();
			InitializeCategoryMenu();
			InitializeInProgressUpgrades();
			isOpen = true;
		}
		else
		{
			Close();
		}
	}

	public void Close()
	{
		HexGameUI.menuOpen = false;
		upgradeClassName = null;
		upgradeValues = new TechTree.Item();
		gameObject.SetActive(false);
		ClearMenu();
		isOpen = false;
	}

	private void InitializeCategoryMenu()
	{
		string[] buttonNames = { "Units", "Buildings", "Resources" };

		if (!listContent.GetComponent<VerticalLayoutGroup>())
		{
			AddVerticalLayoutGroup();
		}

		for (int i = 0; i < 3; i++)
		{
			UITechCategoryButton button = Instantiate(techCategoryPrefab);
			button.category = buttonNames[i].ToLower();
			button.buttonText.text = buttonNames[i];
			button.techMenu = this;
			button.transform.SetParent(listContent.transform, false);
		}
	}

	public void InitializeObjectMenu(string objectType)
	{
		ClearMenu();

		switch (objectType)
		{
			case "buildings":
				for (int i = 0; i < buildingsRoot.buildingsCollection.prefabs.Length; i++)
				{
					UITechObjectButton button = Instantiate(techObjectPrefab);
					button.objectName = buildingsRoot.buildingsCollection.PickBuilding(i).buildingName;
					button.buttonText.text = button.objectName;
					button.techMenu = this;
					button.transform.SetParent(listContent.transform, false);
				}
				break;
			case "units":
				for (int i = 0; i < unitsRoot.unitsCollection.prefabs.Length; i++)
				{
					UITechObjectButton button = Instantiate(techObjectPrefab);
					button.objectName = unitsRoot.unitsCollection.PickUnit(i).unitName;
					button.buttonText.text = button.objectName;
					button.techMenu = this;
					button.transform.SetParent(listContent.transform, false);
				}
				break;
			case "resources":
				for (int i = 0; i < resourcesRoot.resourcesCollection.prefabs.Length; i++)
				{
					UITechObjectButton button = Instantiate(techObjectPrefab);
					button.objectName = resourcesRoot.resourcesCollection.PickResource(i).ResourceName;
					button.buttonText.text = button.objectName;
					button.techMenu = this;
					button.transform.SetParent(listContent.transform, false);
				}
				break;
			default:
				Debug.Log("Invalid upgrade object supplied");
				break;
		}
	}

	public void InitializeSpecificUpgradesMenu(string objectName)
	{
		ClearMenu();
		objectName = objectName.ToLower();

		switch (objectName)
		{
			case "builder":
				//InitializeBuilderUpgrades();
				InitializeUpgrades(TechTree.Units.Builder.upgrades);
				break;
			case "harvester":
				//InitializeHarvesterUpgrades();
				InitializeUpgrades(TechTree.Units.Harvester.upgrades);
				break;
			case "crystal":
				InitializeUpgrades(TechTree.Resources.Crystal.upgrades);
				break;
			default:
				Debug.Log("not ready yet");
				break;
		}
	}

	private void InitializeUpgrades(TechTree.Upgrade[] upgrades)
	{
		ClearMenu();

		foreach (TechTree.Upgrade upgrade in upgrades)
		{
			bool isAvailable = false;

			UITechUpgradeButton button = Instantiate(techUpgradePrefab);
			button.buttonText.text = upgrade.upgradeName;

			foreach (TechTree.Item uItem in upgrade.upgradeItems)
			{
				if (!uItem.applied)
				{
					button.upgrade = uItem;
					isAvailable = true;
					break;
				}
			}

			button.techMenu = this;

			if (!isAvailable)
			{
				Destroy(button);
			}
			else
			{
				button.transform.SetParent(listContent.transform, false);
			}
		}
	}

	public void InitializeInProgressUpgrades()
	{
		try
		{
			ResearchItemInProgress[] researchItems = techRoot.gameObject.GetComponents<ResearchItemInProgress>();

			foreach (ResearchItemInProgress researchItem in researchItems)
			{
				UITechUpgradeButton button = Instantiate(techUpgradePrefab);
				button.isInQueue = true;
				button.techMenu = this;
				button.buttonText.text = researchItem.upgradeItem.itemName;
				button.researchItem = researchItem;
				button.upgrade = researchItem.upgradeItem;
				button.InitializeTechUpgradeButton();
				button.transform.SetParent(queueListContent.transform, false);
			}
		}
		catch
		{
			Debug.Log("error with research items in progress, could be none in progress");
		}
	}

	private void ClearMenu()
	{
		for (int i = 0; i < listContent.childCount; i++)
		{
			Destroy(listContent.GetChild(i).gameObject);
		}

		for (int i = 0; i < queueListContent.childCount; i++)
		{
			Destroy(queueListContent.GetChild(i).gameObject);
		}
		//RemoveVerticalLayoutGroup();
	}

	public void ClearQueueMenu()
	{
		for (int i = 0; i < queueListContent.childCount; i++)
		{
			Destroy(queueListContent.GetChild(i).gameObject);
		}
	}

	public void TurnOnUpgradeItems(TechTree.Item itemValues)
	{
		upgradeTitle.gameObject.SetActive(true);
		upgradeTitle.text = itemValues.itemName;

		upgradeDescription.gameObject.SetActive(true);
		upgradeDescription.text = itemValues.description;

		upgradeCost.gameObject.SetActive(true);
		upgradeCost.text = itemValues.cost.ToString();

		upgradeButton.gameObject.SetActive(true);
	}

	private void TurnOffUpgradeItems()
	{
		upgradeTitle.gameObject.SetActive(false);
		upgradeDescription.gameObject.SetActive(false);
		upgradeCost.gameObject.SetActive(false);
		upgradeButton.gameObject.SetActive(false);
	}

	private void AddVerticalLayoutGroup()
	{
		VerticalLayoutGroup layout = listContent.gameObject.AddComponent<VerticalLayoutGroup>();
		layout.padding = new RectOffset(10, 10, 10, 10);
		layout.spacing = 10.0f;
		layout.childScaleHeight = false;
		layout.childControlHeight = true;

		//listContent.gameObject.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
		//listContent.gameObject.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
	}

	private void RemoveVerticalLayoutGroup()
	{
		Destroy(listContent.gameObject.GetComponent<VerticalLayoutGroup>());
	}

	public void TryUpgrade()
	{
		if (Economy.Dollars >= upgradeValues.cost)
		{
			DoUpgrade();
		}
		else
		{
			ShowErrorPanel();
		}
	}

	//public void DoUpgrade()
	//{
	//    bool applied = false;

	//    switch (upgradeClassName)
	//    {
	//        case "harvester":
	//            UnitHarvester.DoUpgrade(upgradeValues.itemName, upgradeValues.value);
	//            applied = true;
	//            break;
	//        case "builder":
	//            UnitBuilder.DoUpgrade(upgradeValues.itemName, upgradeValues.value);
	//            applied = true;
	//            break;
	//        case "crystal":
	//            ResourceBaseClass.DoUpgrade(upgradeValues.itemName, upgradeValues.value);
	//            applied = true;
	//            break;
	//        default:
	//            break;
	//    }

	//    if (applied)
	//    {
	//        upgradeValues.applied = true;
	//        Economy.Dollars -= upgradeValues.cost;
	//    }
	//    else
	//    {
	//        Debug.Log("error applying upgrade and applying cost");
	//    }

	//    Close();
	//}

	public void DoUpgrade()
	{
		ResearchItemInProgress researchItem = techRoot.gameObject.AddComponent<ResearchItemInProgress>();
		researchItem.upgradeClassName = upgradeClassName;
		researchItem.upgradeItem = upgradeValues;
		researchItem.isInitialized = true;
		researchItem.techMenu = this;
		Economy.Dollars -= upgradeValues.cost;


		ClearQueueMenu();
		InitializeInProgressUpgrades();
	}

	public void ShowErrorPanel()
	{
		errorPanel.SetActive(true);
	}

	public void CloseErrorPanel()
	{
		errorPanel.SetActive(false);
	}
}
