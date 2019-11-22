using UnityEngine;

[System.Serializable]
public struct HexFeatureCollection
{
    public Transform[] prefabs;

    public Transform Pick (float choice)
    {
        return prefabs[(int)choice * prefabs.Length];
    }

    public Transform PickFirst ()
    {
        return prefabs[0];
    }

    public HexUnit PickUnit (int choice)
    {
        return prefabs[choice].GetComponent<HexUnit>();
    }

    public ResourceBaseClass PickResource (int choice)
    {
        return prefabs[choice].GetComponent<ResourceBaseClass>();
    }

    public BuildingBaseClass PickBuilding (int choice)
    {
        return prefabs[choice].GetComponent<BuildingBaseClass>();
    }
}
