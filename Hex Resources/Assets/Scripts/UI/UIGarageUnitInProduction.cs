using UnityEngine;
using System.Collections;

public class UIGarageUnitInProduction : MonoBehaviour
{
    public HexUnit unitToBuild;

    private float productionTimer;

    private float productionPercentage;

    public UIGarageMenu garageMenu;

    public BuildingGarage buildingGarage;

    public HexGrid grid;

    public bool initialized;

    public bool inProduciton;

    private void Start()
    {
        ResetProductionTimer();
    }

    private void ResetProductionTimer()
    {
        productionTimer = 0.0f;
        productionPercentage = 0.0f;
    }

    public void StartProduction()
    {
        StartCoroutine(ProduceUnit());
    }

    public void StopProduction()
    {
        StopAllCoroutines();
    }

    private IEnumerator ProduceUnit()
    {
        try
        {
            garageMenu.ClearAvailableUnitsList();
            garageMenu.InitializeUnitsToBuildView();
            garageMenu.InitializeUnitsQueue();
        }
        catch
        {
            Debug.Log("garage menu closed");
        }

        inProduciton = true;
        while (productionTimer < unitToBuild.ProductionTime)
        {
            productionTimer += Time.deltaTime;
            productionPercentage = productionTimer / unitToBuild.UnitBuildTime;
            yield return null;
        }

        BuildUnitCoroutine();
    }

    public void BuildUnitCoroutine()
    {
        HexUnit unit = Instantiate(unitToBuild);
        grid.AddUnit(unit, buildingGarage.buildDestination, buildingGarage.orientation);
        unit.InitializeSpawn(buildingGarage.Location, buildingGarage.buildDestination);
        Destroy(this);
    }

    public float GetProgress()
    {
        return productionPercentage;
    }
}
