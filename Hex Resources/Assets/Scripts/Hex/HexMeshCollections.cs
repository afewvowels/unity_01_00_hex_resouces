using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMeshCollections : MonoBehaviour
{
    [SerializeField]
    public static List<UnitMesh> units = new List<UnitMesh>();
    [SerializeField]
    public static List<BuildingMesh> buildings = new List<BuildingMesh>();
    [SerializeField]
    public static List<ResourceMesh> resources = new List<ResourceMesh>();

    private void Awake()
    {
    }
}
