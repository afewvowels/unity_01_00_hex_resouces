using System.Collections;
using System;
using UnityEngine;

public class ResourceMesh : IComparable<ResourceMesh>
{
    private int id;
    private GameObject resource;

    public ResourceMesh(int newID, GameObject newResource)
    {
        id = newID;
        resource = newResource;
    }

    public int CompareTo (ResourceMesh other)
    {
        if (other == null)
        {
            return 1;
        }

        return id - other.id;
    }

    public GameObject GetResourceGameObject
    {
        get
        {
            return resource;
        }
        set
        {
            if (value != null)
            {
                resource = value;
            }
        }
    }
}
