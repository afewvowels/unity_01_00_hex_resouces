using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BuildingBaseClass : MonoBehaviour
{
    [SerializeField]
    protected int health = 100;

    public int buildingID;

    [SerializeField]
    protected HexCell location;

    public HexGrid Grid { get; set; }

    protected const int visionRange = 3;

    public string buildingName;

    public float orientation;

    public int resourceAmount;

    public virtual bool IsAvailable { get; set; }

    public bool IsBusy { get; set; }

    public int ResourceAmount
    {
        get
        {
            return resourceAmount;
        }
        set
        {
            resourceAmount = value;
            Economy.Crystals = resourceAmount;
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
            orientation = value;
            transform.localRotation = Quaternion.Euler(0.0f, value, 0.0f);
        }
    }

    public int BuildingID
    {
        get
        {
            return buildingID;
        }
        set
        {
            buildingID = value;
        }
    }

    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health -= value;
            if (health <= 0)
            {
                Die();
            }
        }
    }

    public int VisionRange
    {
        get
        {
            return visionRange;
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
            if (location)
            {
                Grid.DecreaseVisibility(location, visionRange);
                location.Building = null;
            }
            location = value;
            value.Building = this;
            Grid.IncreaseVisibility(location, visionRange);
            transform.localPosition = value.Position;
            Grid.MakeChildOfColumn(transform, value.ColumnIndex);
        }
    }

    public void Die()
    {
        if (location)
        {
            Grid.DecreaseVisibility(location, visionRange);
        }
        location.Building = null;
        Destroy(gameObject);
    }

    public virtual void ResourceBuildingAction()
    {
        Debug.Log("called virtual method");
    }

    public virtual void StartBuildMe()
    {
        StartCoroutine(BuildMe());
    }

    public virtual void StartBuildMe(Color[] colorsArr)
    {
        StartCoroutine(BuildMe(colorsArr));
    }

    public virtual IEnumerator BuildMe()
    {
        yield return null;
    }

    public virtual IEnumerator BuildMe(Color[] colorsArr)
    {
        yield return null;
    }
}
