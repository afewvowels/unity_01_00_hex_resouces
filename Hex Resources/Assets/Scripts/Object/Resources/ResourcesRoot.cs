using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesRoot : MonoBehaviour
{
    public static List<ResourceBaseClass> resourcesGlobalList = new List<ResourceBaseClass>();

    public HexGrid grid;

    public ResourceBaseClass[] resources;

    public HexFeatureCollection resourcesCollection;

    private void Start()
    {
        grid = GameObject.FindGameObjectWithTag("hexgrid").GetComponent<HexGrid>();
        //RefreshResourcesList();
    }

    private void RefreshResourcesList()
    {
        GameObject[] resourcesBase = GameObject.FindGameObjectsWithTag("resource");
        resources = new ResourceBaseClass[resourcesBase.Length];

        for (int i = 0; i < resourcesBase.Length; i++)
        {
            resources[i] = resourcesBase[i].GetComponent<ResourceBaseClass>();
        }
    }

    //public List<HexCell> GetNearestResourcePath(HexCell origin)
    //{
    //    RefreshResourcesList();
    //    List<HexCell> shortestPath = new List<HexCell>();

    //    foreach (ResourceBaseClass resource in resources)
    //    {
    //        if (resource.ResourceAmount > 25)
    //        {
    //            grid.FindPath(origin, resource.Location, 10, false);
    //            List<HexCell> tempPath = grid.GetPath();

    //            if (tempPath != null)
    //            {
    //                if (tempPath.Count < shortestPath.Count ||
    //                    shortestPath.Count == 0)
    //                {
    //                    shortestPath = tempPath;
    //                }
    //            }
    //        }
    //    }

    //    return shortestPath;
    //}

    public List<HexCell> GetNearestResourcePath(HexCell origin)
    {
        List<HexCell> shortestPath = new List<HexCell>();

        foreach (ResourceBaseClass resource in resourcesGlobalList)
        {
            if (resource.ResourceAmount > 25)
            {
                grid.FindPath(origin, resource.Location, 10, false);
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
        }

        return shortestPath;
    }
}
