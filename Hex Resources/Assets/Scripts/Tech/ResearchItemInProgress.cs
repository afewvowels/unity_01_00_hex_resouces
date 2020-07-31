using System.Collections;
using UnityEngine;

public class ResearchItemInProgress : MonoBehaviour
{
	public TechTree.Item upgradeItem;

	public string upgradeClassName;

	public float productionTime;
	public float productionProgress;

	public bool isInitialized;

	public bool isResearching;

	public UITechMenu techMenu;

	private void Start()
	{
		ResetTimers();
	}

	private void FixedUpdate()
	{

	}

	public void StartResearchItem()
	{
		StartCoroutine(DoResearch());
	}

	private void ResetTimers()
	{
		productionTime = 0.0f;
		productionProgress = 0.0f;
	}

	private IEnumerator DoResearch()
	{
		try
		{
			techMenu.ClearQueueMenu();
			techMenu.InitializeInProgressUpgrades();
		}
		catch
		{
			Debug.Log("tech menu closed");
		}

		isResearching = true;
		while (productionTime < upgradeItem.upgradeTime)
		{
			productionTime += Time.deltaTime;
			productionProgress = productionTime / upgradeItem.upgradeTime;

			yield return null;
		}

		bool applied = false;

		switch (upgradeClassName)
		{
			case "harvester":
				UnitHarvester.DoUpgrade(upgradeItem.itemName, upgradeItem.value);
				applied = true;
				break;
			case "builder":
				UnitBuilder.DoUpgrade(upgradeItem.itemName, upgradeItem.value);
				applied = true;
				break;
			case "crystal":
				ResourceBaseClass.DoUpgrade(upgradeItem.itemName, upgradeItem.value);
				applied = true;
				break;
			default:
				break;
		}

		if (applied)
		{
			upgradeItem.applied = true;
			//Economy.Dollars -= upgradeItem.cost;
		}
		else
		{
			Economy.Dollars += upgradeItem.cost;
			Debug.Log("error applying upgrade and applying cost");
		}

		Destroy(this);
	}
}
