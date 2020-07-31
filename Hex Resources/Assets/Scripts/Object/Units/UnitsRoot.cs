using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitsRoot : MonoBehaviour
{
    public HexGrid grid;

    [SerializeField]
    private HexUnit selectedUnit;

    [SerializeField]
    private HexCell currentCell;

    public HexFeatureCollection unitsCollection;

    public BuildingsRoot buildingsRoot;

    public void CreateUnit(HexUnit unit)
    {
        bool isSet = false;
        // bool isValid = true;
        while (!isSet)
        {
            HexCell cell = grid.GetRandomHexCell();
            if (!cell.IsUnderwater && !cell.Unit && !cell.HasResource && cell.Explorable && !cell.Building && !cell.HasRiver)
            {
                // foreach (HexCell neighbor in cell.GetNeighbors())
                // {
                //     if (cell.IsUnderwater && cell.Unit && cell.HasResource && !cell.Explorable && cell.Building && cell.HasRiver)
                //     {
                //         isValid = false;
                //     }
                // }
                grid.ValidateStartPosition(cell);
                if (grid.isValidStartPosition)
                {
                    if (unit.GetComponent<UnitBuilder>())
                    {
                        unit.GetComponent<UnitBuilder>().unitsRoot = this;
                        unit.GetComponent<UnitBuilder>().buildingsRoot = buildingsRoot;
                    }
                    grid.AddUnit(Instantiate(unit), cell, Random.Range(0.0f, 360.0f));
                    isSet = true;
                }

                // if (isValid)
                // {
                //     if (unit.GetComponent<UnitBuilder>())
                //     {
                //         unit.GetComponent<UnitBuilder>().unitsRoot = this;
                //         unit.GetComponent<UnitBuilder>().buildingsRoot = buildingsRoot;
                //     }
                //     grid.AddUnit(Instantiate(unit), cell, Random.Range(0.0f, 360.0f));
                //     isSet = true;
                // }
            }
        }
    }
}
