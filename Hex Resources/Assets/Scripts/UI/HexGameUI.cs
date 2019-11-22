using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HexGameUI : MonoBehaviour
{
    public HexGrid grid;

    public HexCell currentCell;

    [SerializeField]
    public HexUnit selectedUnit;

    [SerializeField]
    public BuildingBaseClass selectedBuilding;

    public UISelectedMenu selectedMenu;

    public UISecretMenu secretMenu;

    private void FixedUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject() && !IsPointerOverUIElement())
        {
            if (Input.GetMouseButtonDown(0) && selectedMenu.placeBuilding != true)
            {
                DoSelection();
            }
            else if (selectedMenu.placeBuilding)
            {
                grid.ClearPath();
            }
            else if (selectedUnit && selectedMenu.placeBuilding != true)
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
        else if (Input.GetKeyDown(KeyCode.P))
        {
            if (!secretMenu.isOpen)
            {
                secretMenu.Close();
            }
            else
            {
                secretMenu.Open();
            }
        }
    }

    public void SetEditMode (bool toggle)
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
        grid.ClearPath();
        UpdateCurrentCell();
        if (currentCell.Unit)
        {
            selectedUnit = currentCell.Unit;
            selectedBuilding = null;

            selectedMenu.Open();
        }
        else if (currentCell.Building)
        {
            selectedBuilding = currentCell.Building;
            selectedUnit = null;

            selectedMenu.Open();
        }
        else
        {
            selectedMenu.Close();
        }
    }

    private void DoPathfinding()
    {
        if (UpdateCurrentCell())
        {
            if (currentCell && selectedUnit.IsValidDestination(currentCell))
            {
                grid.FindPath(selectedUnit.Location, currentCell, 24);
            }
            else
            {
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
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults )
    {
        for(int index = 0;  index < eventSystemRaysastResults.Count; index ++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults [index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("selectedui"))
                return true;
        }
        return false;
    }
    ///Gets all event systen raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {   
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position =  Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll( eventData, raysastResults );
        return raysastResults;
    }
}
