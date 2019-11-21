using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;

    public RectTransform uiRect;

    [SerializeField]
    private int terrainTypeIndex;

    private int distance;

	[SerializeField]
	private int parentCellID;

	[SerializeField]
	int cellID;

	[SerializeField]
	HexCell[] neighbors = null;

	[SerializeField]
	private int elevation = int.MinValue;

    [SerializeField]
    private bool[] roads = null;

    public int SearchPhase { get; set; }
    public int Index { get; set; }
    public bool Explorable { get; set; }

    [SerializeField]
    private int waterLevel = int.MinValue;

    public HexGridChunk chunk;

    private bool hasIncomingRiver, hasOutgoingRiver, explored;

    private int visibility;

    [SerializeField]
    private HexUnit unit;

    [SerializeField]
    private BuildingBaseClass building;

    [SerializeField]
    private HexDirection incomingRiverDirection, outgoingRiverDirection;

    [SerializeField]
    public HexCell PathFrom { get; set; }

    [SerializeField]
    public int SearchHeuristic { get; set; }

    public HexCell NextWithSamePriority { get; set; }

    public BuildingBaseClass Building
    {
        get
        {
            return building;
        }
        set
        {
            building = value;
        }
    }

    public HexUnit Unit
    {
        get
        {
            return unit;
        }
        set
        {
            unit = value;
        }
    }

    public HexCellShaderData ShaderData { get; set; }

    public int ColumnIndex { get; set; }

    [SerializeField]
    private ResourceBaseClass resource;

    public ResourceBaseClass Resource
    {
        get
        {
            return resource;
        }
        set
        {
            resource = value;
        }
    }

    public bool HasResource
    {
        get
        {
            return Resource != null;
        }
    }

    private int resourceType = int.MinValue;

    public int ResourceType
    {
        get
        {
            return resourceType;
        }
        set
        {
            resourceType = value;
        }
    }

    public bool IsExplored
    {
        get
        {
            return explored && Explorable;
        }
        private set
        {
            explored = value;
        }
    }

    private int urbanLevel;
    private int farmLevel;
    private int plantLevel;
    private int specialIndex;

    private bool walled;

    public bool IsVisible
    {
        get
        {
            return visibility > 0 && Explorable;
        }
    }

    public int SpecialIndex
    {
        get
        {
            return specialIndex;
        }
        set
        {
            if (specialIndex != value && !HasRiver)
            {
                specialIndex = value;
                RemoveRoads();
                RefreshSelfOnly();
            }
        }
    }

    public bool IsSpecial
    {
        get
        {
            return specialIndex > 0;
        }
    }

    public bool Walled
    {
        get
        {
            return walled;
        }
        set
        {
            if (walled != value)
            {
                walled = value;
                Refresh();
            }
        }
    }

    public int UrbanLevel
    {
        get
        {
            return urbanLevel;
        }
        set
        {
            if (urbanLevel != value)
            {
                urbanLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    public int FarmLevel
    {
        get
        {
            return farmLevel;
        }
        set
        {
            if (farmLevel != value)
            {
                farmLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    public int PlanetLevel
    {
        get
        {
            return plantLevel;
        }
        set
        {
            if (plantLevel != value)
            {
                plantLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    public int SearchPriority
    {
        get
        {
            return distance + SearchHeuristic;
        }
    }

    public int Distance
    {
        get
        {
            return distance;
        }
        set
        {
            distance = value;
        }
    }

	public int Elevation
	{
		get
		{
			return elevation;
		}
		set
		{
            if (elevation == value)
            {
                return;
            }
            int originalViewElevation = ViewElevation;
			elevation = value;
            if (ViewElevation != originalViewElevation)
            {
                ShaderData.ViewElevationChanged();
            }


            RefreshPosition();
            ValidateRivers();

            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i] && GetElevationDifference((HexDirection)i) > 1)
                {
                    SetRoad(i, false);
                }
            }

            Refresh();
		}
	}

    public int ViewElevation
    {
        get
        {
            return elevation >= waterLevel ? elevation : waterLevel;
        }
    }

    public int TerrainTypeIndex
    {
        get
        {
            return terrainTypeIndex;
        }
        set
        {
            if (terrainTypeIndex != value)
            {
                terrainTypeIndex = value;
                ShaderData.RefreshTerrain(this);
            }
        }
    }

	public Vector3 Position
	{
		get
		{
			return transform.localPosition;
		}
	}

    public bool HasIncomingRiver
    {
        get
        {
            return hasIncomingRiver;
        }
    }

    public bool HasOutgoingRiver
    {
        get
        {
            return hasOutgoingRiver;
        }
    }

    public HexDirection IncomingRiverDirection
    {
        get
        {
            return incomingRiverDirection;
        }
    }

    public HexDirection OutgoingRiverDirection
    {
        get
        {
            return outgoingRiverDirection;
        }
    }

    public bool HasRiver
    {
        get
        {
            return hasIncomingRiver || hasOutgoingRiver;
        }
    }

    public bool HasRiverBeginOrEnd
    {
        get
        {
            return hasIncomingRiver != hasOutgoingRiver;
        }
    }

    public HexDirection RiverBeginOrEndDirection
    {
        get
        {
            return hasIncomingRiver ? incomingRiverDirection : outgoingRiverDirection;
        }
    }

    public float StreamBedY
    {
        get
        {
            return (elevation + HexDefinition.streamBedElevationOffset) * HexDefinition.elevationStep;
        }
    }

    public float RiverSurfaceY
    {
        get
        {
            return (elevation + HexDefinition.waterElevationOffset) *
                HexDefinition.elevationStep;
        }
    }

    [SerializeField]
    public float WaterSurfaceY
    {
        get
        {
            return (waterLevel + HexDefinition.waterElevationOffset) *
                HexDefinition.elevationStep;
        }
    }

    public bool HasRoads
    {
        get
        {
            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i])
                {
                    return true;
                }
            }
            return false;
        }
    }

    public int WaterLevel
    {
        get
        {
            return waterLevel;
        }
        set
        {
            if (waterLevel == value)
            {
                return;
            }
            int originalViewElevation = ViewElevation;
            waterLevel = value;

            if (ViewElevation != originalViewElevation)
            {
                ShaderData.ViewElevationChanged();
            }

            ValidateRivers();
            Refresh();
        }
    }

    public bool IsUnderwater
    {
        get
        {
            return waterLevel > elevation;
        }
    }

	private void Awake()
	{

	}

    public void RefreshPosition()
    {
        Vector3 position = transform.localPosition;
        position.y = elevation * HexDefinition.elevationStep;
        position.y += (HexDefinition.SampleNoise(position).y * 2.0f - 1.0f) * HexDefinition.elevationDisplacementStrength;
        transform.localPosition = position;

        if (uiRect)
        {
            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = - position.y;
            uiRect.localPosition = uiPosition;
        }

    }

	public HexCell GetNeighbor(HexDirection direction)
	{
		return neighbors[(int)direction];
	}

	public HexCell[] GetNeighbors()
	{
		return neighbors;
	}

	public void SetNeighbor(HexDirection direction, HexCell cell)
	{
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public void SetCellID(int id)
	{
		cellID = id;
	}

	public int GetCellID()
	{
		return cellID;
	}

	public void SetParentCellID(int id)
	{
		parentCellID = id;
	}

	public int GetParentCellID()
	{
		return parentCellID;
	}

	public HexDefinition.HexEdgeType GetEdgeType(HexDirection direction)
	{
		return HexDefinition.GetEdgeType(elevation, neighbors[(int)direction].elevation);
	}

	public HexDefinition.HexEdgeType GetEdgeType(HexCell otherCell)
	{
		return HexDefinition.GetEdgeType(elevation, otherCell.elevation);
	}

    private void Refresh()
    {
        if(chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
            if (unit)
            {
                unit.ValidatePosition();
            }
        }
    }

    public void RemoveOutgoingRiver()
    {
        if (!hasOutgoingRiver)
        {
            return;
        }
        hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(outgoingRiverDirection);
        neighbor.hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver)
        {
            return;
        }
        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(incomingRiverDirection);
        neighbor.hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void SetOutgoingRiver(HexDirection direction)
    {
        if (hasOutgoingRiver && outgoingRiverDirection == direction)
        {
            return;
        }

        HexCell neighbor = GetNeighbor(direction);
        if (!IsValidRiverDestination(neighbor))
        {
            return;
        }

        RemoveOutgoingRiver();
        if (hasIncomingRiver && incomingRiverDirection == direction)
        {
            RemoveIncomingRiver();
        }

        hasOutgoingRiver = true;
        outgoingRiverDirection = direction;
        specialIndex = 0;

        neighbor.RemoveIncomingRiver();
        neighbor.hasIncomingRiver = true;
        neighbor.incomingRiverDirection = direction.Opposite();
        neighbor.specialIndex = 0;

        SetRoad((int)direction, false);
    }

    private void RefreshSelfOnly()
    {
        chunk.Refresh();
        if (unit)
        {
            unit.ValidatePosition();
        }
    }

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return
            hasIncomingRiver && incomingRiverDirection == direction ||
            hasOutgoingRiver && outgoingRiverDirection == direction;
    }

    public bool HasRoadThroughEdge (HexDirection direction)
    {
        return roads[(int)direction];
    }

    public void RemoveRoads()
    {
        for (int i = 0; i < roads.Length; i++)
        {
            if (roads[i])
            {
                SetRoad(i, false);
            }
        }
    }

    public void AddRoad(HexDirection direction)
    {
        if (!roads[(int)direction] && !HasRiverThroughEdge(direction) &&
            !IsSpecial && !GetNeighbor(direction).IsSpecial &&
            GetElevationDifference(direction) <= 1)
        {
            SetRoad((int)direction, true);
        }
    }

    public void SetRoad(int index, bool state)
    {
        roads[index] = state;
        neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
        neighbors[index].RefreshSelfOnly();
        RefreshSelfOnly();
    }

    public int GetElevationDifference (HexDirection direction)
    {
        int difference = elevation - GetNeighbor(direction).elevation;
        return difference >= 0 ? difference : -difference;
    }

    private bool IsValidRiverDestination (HexCell neighbor)
    {
        return neighbor && (
            elevation >= neighbor.elevation || waterLevel == neighbor.elevation);
    }

    private void ValidateRivers()
    {
        if(
            hasOutgoingRiver &&
            !IsValidRiverDestination(GetNeighbor(outgoingRiverDirection)))
        {
            RemoveOutgoingRiver();
        }
        if(
            hasIncomingRiver &&
            !GetNeighbor(incomingRiverDirection).IsValidRiverDestination(this)
            )
        {
            RemoveIncomingRiver();
        }
    }

    public void Save (BinaryWriter writer)
    {
        writer.Write((byte)terrainTypeIndex);
        writer.Write((byte)(elevation + 127));
        writer.Write((byte)waterLevel);
        writer.Write((byte)urbanLevel);
        writer.Write((byte)farmLevel);
        writer.Write((byte)plantLevel);
        writer.Write((byte)specialIndex);
        writer.Write(walled);

        if (hasIncomingRiver)
        {
            writer.Write((byte)(incomingRiverDirection + 128));
        }
        else
        {
            writer.Write((byte)0);
        }

        if (hasOutgoingRiver)
        {
            writer.Write((byte)(outgoingRiverDirection + 128));
        }
        else
        {
            writer.Write((byte)0);
        }

        int roadFlags = 0;
        for (int i = 0; i < roads.Length; i++)
        {
            if (roads[i])
            {
                roadFlags |= 1 << i;
            }
        }

        writer.Write((byte)roadFlags);
        writer.Write(IsExplored);
    }

    public void Load (BinaryReader reader, int header)
    {
        terrainTypeIndex = reader.ReadByte();
        ShaderData.RefreshTerrain(this);
        elevation = reader.ReadByte();
        if (header >= 4)
        {
            elevation -= 127;
        }
        RefreshPosition();
        waterLevel = reader.ReadByte();
        urbanLevel = reader.ReadByte();
        farmLevel = reader.ReadByte();
        plantLevel = reader.ReadByte();
        specialIndex = reader.ReadByte();
        walled = reader.ReadBoolean();

        byte riverData = reader.ReadByte();
        if (riverData >= 128)
        {
            hasIncomingRiver = true;
            incomingRiverDirection = (HexDirection)(riverData - 128);
        }
        else
        {
            hasIncomingRiver = false;
        }

        riverData = reader.ReadByte();
        if (riverData >= 128)
        {
            hasOutgoingRiver = true;
            outgoingRiverDirection = (HexDirection)(riverData - 128);
        }
        else
        {
            hasOutgoingRiver = false;
        }

        int roadFlags = reader.ReadByte();

        for (int i = 0; i < roads.Length; i++)
        {
            roads[i] = (roadFlags & (1 << i)) != 0;
        }

        IsExplored = header >= 3 ? reader.ReadBoolean() : false;
        ShaderData.RefreshVisibility(this);
    }

    public void SetLabel (string text)
    {
        UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
        label.text = text;
    }

    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.enabled = true;
    }

    public void DisableHighlight()
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.enabled = false;
    }

    public void IncreaseVisibility()
    {
        visibility += 1;
        if (visibility == 1)
        {
            IsExplored = true;
            ShaderData.RefreshVisibility(this);
        }
    }

    public void DecreaseVisibility()
    {
        visibility -= 1;
        if (visibility == 0)
        {
            ShaderData.RefreshVisibility(this);
        }
    }

    public void ResetVisibility()
    {
        if (visibility > 0)
        {
            visibility = 0;
            ShaderData.RefreshVisibility(this);
        }
    }

    public void SetMapData (float data)
    {
        ShaderData.SetMapData(this, data);
    }
}
