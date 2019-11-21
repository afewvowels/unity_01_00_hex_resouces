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
    private int maxResourceAmount = 200;

    //[SerializeField]
    private int resourceRefreshRate = 5;

    public HexGrid Grid { get; set; }

    public int ResourceID { get; set; }

    private string resourceName;

    private float orientation;

    public GameObject harvestParticlesPrefab;

    private GameObject harvestParticles;

    public int ResourceAmount
    {
        get
        {
            return resourceAmount;
        }
        set
        {
            if (value > 0 || value < maxResourceAmount)
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
            StartCoroutine(RefreshResources());
            ResizeResourceBasedOnAmount();
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

    public void Load (BinaryReader reader, HexGrid grid)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        orientation = reader.ReadSingle();
    }

    public void StartHarvest()
    {
        harvestParticles = Instantiate(harvestParticlesPrefab);
        harvestParticles.transform.localPosition = transform.position;
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
        Destroy(harvestParticles);
    }

    private IEnumerator RefreshResources()
    {
        yield return new WaitForSeconds(1.0f);
        while (ResourceAmount < maxResourceAmount)
        {
            ResourceAmount += resourceRefreshRate;
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void ResizeResourceBasedOnAmount()
    {
        float scaleFactor;
        if (ResourceAmount >= maxResourceAmount)
        {
            scaleFactor = 1.0f;
        }
        else if (ResourceAmount > 80)
        {
            scaleFactor = 0.8f;
        }
        else if (ResourceAmount > 60)
        {
            scaleFactor = 0.6f;
        }
        else if (ResourceAmount > 40)
        {
            scaleFactor = 0.4f;
        }
        else if (ResourceAmount > 20)
        {
            scaleFactor = 0.2f;
        }
        else
        {
            scaleFactor = 0.05f;
        }


        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}
