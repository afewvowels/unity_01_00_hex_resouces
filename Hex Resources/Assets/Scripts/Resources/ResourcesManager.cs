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

    public void AddResource (HexCell cell, Vector3 position)
    {
        Transform prefab = resourcesCollection.PickFirst();
        Transform instance = Instantiate(prefab);
        instance.localPosition = HexDefinition.Displace(position);
        instance.localRotation = Quaternion.Euler(0.0f, 360.0f * Random.value, 0.0f);
        instance.SetParent(container, false);
    }
}
