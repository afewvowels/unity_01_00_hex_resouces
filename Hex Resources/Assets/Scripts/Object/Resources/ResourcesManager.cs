using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    public HexFeatureCollection resourcesCollection;

    public Transform container;

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
        ResourceBaseClass resource = Instantiate(resourcesCollection.PickResource(0));
        cell.Resource = resource;
        resource.Location = cell;
        if (resource.transform.localRotation.y <= 1.0f)
        {
            resource.transform.localRotation = Quaternion.Euler(0.0f, 360.0f * Random.value, 0.0f);
        }
        resource.transform.SetParent(container, false);
        //ResourcesRoot.resourcesGlobalList.Add(resource);
    }
}
