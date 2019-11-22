using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingsRoot : MonoBehaviour
{
    public HexGrid grid;

    public BuildingBaseClass[] buildings;

    private void Start()
    {
        grid = GameObject.FindGameObjectWithTag("hexgrid").GetComponent<HexGrid>();
    }

    private void RefreshBuildingsList()
    {
        GameObject[] buildingsBase = GameObject.FindGameObjectsWithTag("building");
        buildings = new BuildingBaseClass[buildingsBase.Length];

        for (int i = 0; i < buildingsBase.Length; i++)
        {
            buildings[i] = buildingsBase[i].GetComponent<BuildingBaseClass>();
        }
    }

    public void CreateBuilding(BuildingBaseClass building, HexUnit unit)
    {
        bool isSet = false;
        while(!isSet)
        {
            HexCell cell = grid.GetRandomHexCell();
            if (!cell.IsUnderwater && !cell.Unit && !cell.HasResource && cell.Explorable && !cell.Building)
            {
                grid.AddBuilding(Instantiate(building), cell);
                foreach (HexCell neighbor in cell.GetNeighbors())
                {
                    if (!neighbor.IsUnderwater && neighbor.Explorable &&
                        !neighbor.Resource && !neighbor.Unit)
                    {
                        grid.AddUnit(Instantiate(unit), neighbor, 150.0f);
                        return;
                    }
                }
                isSet = true;
            }
        }
    }

    public void CreateBuilding(BuildingBaseClass building, HexUnit unit, HexCell location)
    {
        //grid.AddBuilding(Instantiate(building), location);
        foreach (HexCell neighbor in location.GetNeighbors())
        {
            if (!neighbor.IsUnderwater && neighbor.Explorable &&
                !neighbor.Resource && !neighbor.Unit)
            {
                grid.AddUnit(Instantiate(unit), neighbor, 150.0f);
                return;
            }
        }
    }

    public List<HexCell> GetNearestBuildingPath(HexCell origin)
    {
        RefreshBuildingsList();
        List<HexCell> shortestPath = new List<HexCell>();

        foreach (BuildingBaseClass building in buildings)
        {
            grid.FindPath(origin, building.Location, 10, false);
            List<HexCell> tempPath = grid.GetPath();

            if (tempPath != null)
            {
                if (tempPath.Count < shortestPath.Count ||
                    shortestPath.Count == 0)
                {
                    shortestPath = tempPath;
                }
            }
        }

        return shortestPath;
    }
}
