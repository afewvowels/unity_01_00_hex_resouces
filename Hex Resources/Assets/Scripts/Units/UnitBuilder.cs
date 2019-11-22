using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBuilder : HexUnit
{
    public HexCell buildLocation;
    public BuildingsRoot buildingsRoot;
    public UnitsRoot unitsRoot;

    public void BuildAtLocation(BuildingBaseClass building, HexCell goal)
    {
        Grid.FindPath(Location, goal, 10, false);
        TravelToBuildLocation(building, Grid.GetPath());
    }

    public void TravelToBuildLocation (BuildingBaseClass building, List<HexCell> path)
    {
        location.Unit = null;
        location = path[path.Count - 1];
        location.Unit = this;
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPathToBuildLocation(building));
    }

    private IEnumerator TravelPathToBuildLocation(BuildingBaseClass building)
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

        yield return BuildAtLocation(building);
    }

    private IEnumerator BuildAtLocation(BuildingBaseClass building)
    {
        Vector3 noScale = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 fullScaleBuilding = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 fullScaleUnit = new Vector3(2.4f, 2.4f, 2.4f);

        Quaternion fromRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        Quaternion toRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        float rotateStep = 10.0f;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            transform.localScale = Vector3.Lerp(fullScaleUnit, noScale, progress);
            transform.Rotate(0.0f, rotateStep, 0.0f);

            building.transform.localScale = Vector3.Lerp(noScale, fullScaleBuilding, progress);
            building.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, progress);
            
            progress += Time.deltaTime;

            yield return null;
        }

        building.transform.localScale = fullScaleBuilding;

        buildingsRoot.CreateBuilding(building, unitsRoot.unitsCollection.PickUnit(1), Location);

        building.transform.SetParent(buildingsRoot.transform, true);

        building.Location = location;

        Destroy(gameObject);
        yield return null;
    }
}
