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

    public Text economyText;

    public int ResourceAmount
    {
        get
        {
            return resourceAmount;
        }
        set
        {
            resourceAmount = value;
            economyText.text = resourceAmount.ToString();
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

    public virtual HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            if(location)
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

    private void Awake()
    {
        economyText = GameObject.FindGameObjectWithTag("economytext").GetComponent<Text>();
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
}
