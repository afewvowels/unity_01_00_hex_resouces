using UnityEngine;
using UnityEngine.EventSystems;

public class UnitsRoot : MonoBehaviour
{
    public HexUnit unitPrefab;
    public HexGrid grid;

    [SerializeField]
    private HexUnit selectedUnit;

    [SerializeField]
    private HexCell currentCell;

    //private void FixedUpdate()
    //{
    //    if (EventSystem.current.IsPointerOverGameObject())
    //    {
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            DoSelection();
    //        }
    //        else if (selectedUnit)
    //        {
    //            if (Input.GetMouseButtonDown(1))
    //            {
    //                DoMove();
    //            }
    //            else
    //            {
    //                DoPathfinding();
    //            }
    //        }
    //    }
    //}

    //private bool UpdateCurrentCell()
    //{
    //    HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
    //    if (cell != currentCell)
    //    {
    //        currentCell = cell;
    //        return true;
    //    }
    //    return false;
    //}

    //private void DoSelection()
    //{
    //    grid.ClearPath();
    //    UpdateCurrentCell();
    //    if (currentCell)
    //    {
    //        selectedUnit = currentCell.Unit;
    //    }
    //}

    //private void DoPathfinding()
    //{
    //    if (UpdateCurrentCell())
    //    {
    //        if (currentCell && selectedUnit.IsValidDestination(currentCell))
    //        {
    //            grid.FindPath(selectedUnit.Location, currentCell, 24);
    //        }
    //        else
    //        {
    //            grid.ClearPath();
    //        }
    //    }
    //}

    //private void DoMove()
    //{
    //    if (grid.HasPath)
    //    {
    //        selectedUnit.Travel(grid.GetPath());
    //        grid.ClearPath();
    //    }
    //}

    public void CreateUnit(HexUnit unit)
    {
        bool isSet = false;
        while (!isSet)
        {
            HexCell cell = grid.GetRandomHexCell();
            if (!cell.IsUnderwater && !cell.Unit && !cell.HasResource && cell.Explorable && !cell.Building)
            {
                grid.AddUnit(Instantiate(unit), cell, Random.Range(0.0f, 360.0f));
                isSet = true;
            }
        }
    }
}
