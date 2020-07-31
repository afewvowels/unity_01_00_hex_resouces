using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceBaseClass : MonoBehaviour
{
    //[SerializeField]
    private HexCell location;

    //[SerializeField]
    private int resourceAmount;

    //[SerializeField]
    protected static int maxResourceAmount = 50;

    //[SerializeField]
    protected static int resourceRefreshRate = 0;

    public int resonatorRefreshBoost = 0;

    public HexGrid Grid { get; set; }

    public int ResourceID { get; set; }

    public string resourceName;

    private float orientation;

    public GameObject harvestParticlesPrefab;

    private GameObject harvestParticles;

    public bool isRotated;

    public int ResourceAmount
    {
        get
        {
            return resourceAmount;
        }
        set
        {
            if (value > 0 && value < maxResourceAmount)
            {
                resourceAmount = value;
            }
            else if (value >= maxResourceAmount)
            {
                resourceAmount = maxResourceAmount;
            }
            else if (value <= 0)
            {
                resourceAmount = 0;
            }
            else
            {
                Debug.Log("error assigning ResourceAmount");
            }
            StopAllCoroutines();
            try
            {
                StartCoroutine(RefreshResources());
            }
            catch
            {
                Debug.Log("error starting refresh resources coroutine");
            }

            try
            {
                ResizeResourceBasedOnAmount();
            }
            catch
            {
                Debug.Log("error doing resize resource");
            }
        }
    }

    public string ResourceName
    {
        get
        {
            return resourceName;
        }
        set
        {
            resourceName = value;
        }
    }

    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            location = value;
            value.Resource = this;
            transform.localPosition = value.Position;
        }
    }

    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            transform.localRotation = Quaternion.Euler(0.0f, value, 0.0f);
        }
    }

    private void Awake()
    {
        resourceAmount = maxResourceAmount;
    }

    private void OnEnable()
    {
        if (location)
        {
            transform.localPosition = location.Position;
        }
    }

    private void FixedUpdate()
    {
    }

    public void ValidatePosition()
    {
        transform.localPosition = location.Position;
    }

    public void Die()
    {
        location.Resource = null;
        Destroy(gameObject);
    }

    public void Save(BinaryWriter writer)
    {
        location.coordinates.Save(writer);
        writer.Write(orientation);
    }

    public void Load(BinaryReader reader, HexGrid grid)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        orientation = reader.ReadSingle();
    }

    public void StartHarvest()
    {
        harvestParticles = Instantiate(harvestParticlesPrefab);
        harvestParticles.transform.localPosition = transform.position;
        //harvestParticles.transform.SetParent(this.transform, true);
        //harvestParticles.transform.SetParent(transform);
    }

    //public IEnumerator EndHarvest()
    //{
    //    ParticleSystem.EmissionModule emissionModule = harvestParticles.GetComponent<ParticleSystem>().emission;

    //    emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(0);
    //    yield return new WaitForSeconds(0.1f);

    //    Destroy(harvestParticles);
    //}

    public void EndHarvest()
    {
        try
        {
            Destroy(harvestParticles);
        }
        catch
        {
            Debug.Log("error destroying harvest particles");
        }
    }

    private IEnumerator RefreshResources()
    {
        yield return new WaitForSeconds(1.0f);
        while (ResourceAmount < maxResourceAmount)
        {
            ResourceAmount += resourceRefreshRate;
            if (Location.resonator)
            {
                ResourceAmount += BuildingHarmonicResonator.resourceRefreshRateBoost;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    protected virtual void ResizeResourceBasedOnAmount()
    {
        float scaleFactor;
        if (ResourceAmount >= maxResourceAmount)
        {
            scaleFactor = 3.0f;
        }
        else if (ResourceAmount > maxResourceAmount * 0.8f)
        {
            scaleFactor = 2.5f;
        }
        else if (ResourceAmount > maxResourceAmount * 0.6f)
        {
            scaleFactor = 2.0f;
        }
        else if (ResourceAmount > maxResourceAmount * 0.4f)
        {
            scaleFactor = 1.5f;
        }
        else if (ResourceAmount > maxResourceAmount * 0.2f)
        {
            scaleFactor = 1.0f;
        }
        else
        {
            scaleFactor = 0.5f;
        }

        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    public static void DoUpgrade(string propName, int value)
    {
        bool isSet;

        switch (propName.ToLower())
        {
            case "refresh rate":
                resourceRefreshRate = value;
                isSet = true;
                break;
            case "capacity":
                maxResourceAmount = value;
                isSet = true;
                break;
            default:
                isSet = false;
                break;
        }

        if (isSet)
        {
            SetUpgradeActive(propName, value);
        }
        else
        {
            Debug.Log("error setting resource upgrade to applied");
        }
    }

    private static void SetUpgradeActive(string propName, int _value)
    {
        for (int i = 0; i < TechTree.Resources.Crystal.upgrades.Length; i++)
        {
            for (int j = 0; j < TechTree.Resources.Crystal.upgrades[i].upgradeItems.Length; j++)
            {
                if (TechTree.Resources.Crystal.upgrades[i].upgradeItems[j].itemName.ToLower() == propName.ToLower() &&
                    TechTree.Resources.Crystal.upgrades[i].upgradeItems[j].value == _value)
                {
                    TechTree.Resources.Crystal.upgrades[i].upgradeItems[j].applied = true;
                    Debug.Log("Crystal upgrade applied");
                    break;
                }
            }
        }
    }
}
