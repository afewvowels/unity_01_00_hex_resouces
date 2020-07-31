using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    public HexFeatureCollection resourcesCollection;

    public Transform container;

    private HexHash hash;

    private void Start()
    {

    }

    public void Clear()
    {
        if (container)
        {
            Destroy(container.gameObject);
        }
        container = new GameObject("Resources Container").transform;
        container.SetParent(transform, false);
    }

    public void Apply()
    {

    }

    public void AddResource(HexCell cell)
    {
        hash = HexDefinition.SampleHashGrid(cell.Position);
        float usedHash = hash.a;
        ResourceBaseClass resource = Instantiate(resourcesCollection.PickResource(0));
        cell.Resource = resource;
        resource.Location = cell;
        if (!resource.isRotated)
        {
            resource.transform.localRotation = Quaternion.Euler(0.0f, 360.0f * usedHash, 0.0f);
            resource.isRotated = true;
        }
        resource.transform.SetParent(container, false);
        //ResourcesRoot.resourcesGlobalList.Add(resource);
    }
}
