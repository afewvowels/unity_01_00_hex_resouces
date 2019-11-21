using System.Collections;
using System;
using UnityEngine;

public class BuildingMesh : IComparable<BuildingMesh>
{
    private int id;
    private GameObject building;

    public BuildingMesh(int newID, GameObject newBuilding)
    {
        id = newID;
        building = newBuilding;
    }

    public int CompareTo (BuildingMesh other)
    {
        if (other == null)
        {
            return 1;
        }

        return id - other.id;
    }

    public GameObject GetBuildingGameObject
    {
        get
        {
            return building;
        }
        set
        {
            if (value != null)
            {
                building = value;
            }
        }
    }
}
