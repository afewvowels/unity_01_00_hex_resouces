using System.Collections;
using System;
using UnityEngine;

public class UnitMesh : IComparable<UnitMesh>
{
    private int id;
    private GameObject unit;

    public UnitMesh(int newID, GameObject newUnit)
    {
        id = newID;
        unit = newUnit;
    }

    public int CompareTo (UnitMesh other)
    {
        if (other == null)
        {
            return 1;
        }

        return id - other.id;
    }

    public GameObject GetUnitGameObject
    {
        get
        {
            return unit;
        }
        set
        {
            if (value != null)
            {
                unit = value;
            }
        }
    }
}
