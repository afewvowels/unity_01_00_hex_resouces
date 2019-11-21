using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

public class UnitHarvester : HexUnit
{
    [SerializeField]
    private int heldResources;
    private int maxHeldResources = 150;
    private int harvestAmount = 5;

    public ResourcesRoot resourcesRoot;
    public BuildingsRoot buildingsRoot;

    public HexCell destination;

    public bool needsDestination;

    private void Awake()
    {
        needsDestination = true;
        heldResources = 0;
        resourcesRoot = GameObject.FindGameObjectWithTag("resourcesroot").GetComponent<ResourcesRoot>();
        buildingsRoot = GameObject.FindGameObjectWithTag("buildingsroot").GetComponent<BuildingsRoot>();
    }

    void FixedUpdate()
    {
        if (needsDestination)
        {
            needsDestination = false;
            Location = Grid.GetHexCell(HexCoordinates.FromPosition(transform.position));
            if (heldResources < maxHeldResources)
            {
                pathToTravel = resourcesRoot.GetNearestResourcePath(Location);
            }
            else
            {
                pathToTravel = buildingsRoot.GetNearestBuildingPath(Location);
            }

            if (pathToTravel.Count > 2)
            {
                destination = pathToTravel[pathToTravel.Count - 1];
                pathToTravel.RemoveAt(pathToTravel.Count - 1);
                TravelHarvester(pathToTravel);
            }
            else
            {
                destination = IsAdjacentToDestination(Location);
                StartCoroutine(DoAdjacent());
            }
        }
    }


    public void TravelHarvester(List<HexCell> path)
    {
        location.Unit = null;
        location = path[path.Count - 1];
        location.Unit = this;
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPathHarvester());
    }

    private IEnumerator TravelPathHarvester()
    {
        Vector3 a, b, c = pathToTravel[0].Position;
        yield return LookAt(pathToTravel[1].Position);
        if (!currentTravelLocation)
        {
            currentTravelLocation = pathToTravel[0];
        }
        Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
        int currentColumn = currentTravelLocation.ColumnIndex;

        float t = Time.deltaTime * travelSpeed;
        for (int i = 1; i < pathToTravel.Count; i++)
        {
            currentTravelLocation = pathToTravel[i];
            a = c;
            b = pathToTravel[i - 1].Position;

            int nextColumn = currentTravelLocation.ColumnIndex;
            if (currentColumn != nextColumn)
            {
                if (nextColumn < currentColumn - 1)
                {
                    a.x -= HexDefinition.innerDiameter * HexDefinition.wrapSize;
                    b.x -= HexDefinition.innerDiameter * HexDefinition.wrapSize;
                }
                else if (nextColumn > currentColumn + 1)
                {
                    a.x += HexDefinition.innerDiameter * HexDefinition.wrapSize;
                    b.x += HexDefinition.innerDiameter * HexDefinition.wrapSize;
                }
                Grid.MakeChildOfColumn(transform, nextColumn);
                currentColumn = nextColumn;
            }

            c = (b + currentTravelLocation.Position) * 0.5f;
            Grid.IncreaseVisibility(pathToTravel[i], VisionRange);

            for (; t < 1.0f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0.0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            Grid.DecreaseVisibility(pathToTravel[i], visionRange);
            t -= 1.0f;
        }
        currentTravelLocation = null;

        a = c;
        b = location.Position;
        c = b;
        Grid.IncreaseVisibility(location, visionRange);
        for (; t < 1.0f; t += Time.deltaTime * travelSpeed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0.0f;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        transform.localPosition = location.Position;

        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;

        yield return DoAdjacent();
    }

    private IEnumerator DoAdjacent()
    {
        yield return LookAt(destination.transform.position);

        if (destination.Resource)
        {
            destination.Resource.StartHarvest();
            while(heldResources < maxHeldResources && destination.Resource.ResourceAmount != 0)
            {
                destination.Resource.ResourceAmount -= harvestAmount;
                heldResources += harvestAmount;
                yield return new WaitForSeconds(0.1f);
            }
            //StartCoroutine(destination.Resource.EndHarvest());
            destination.Resource.EndHarvest();
        }
        else if (destination.Building)
        {
            while(heldResources > 0)
            {
                heldResources -= harvestAmount;
                destination.Building.ResourceAmount += harvestAmount;
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            Debug.Log("destination not set");
        }

        needsDestination = true;
    }

    private HexCell IsAdjacentToDestination(HexCell origin)
    {
        foreach (HexCell neighbor in origin.GetNeighbors())
        {
            if (neighbor.Resource || neighbor.Building)
            {
                return neighbor;
            }
        }

        return null;
    }
}
