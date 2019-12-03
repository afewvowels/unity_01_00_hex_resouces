using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class UnitSurveyor : HexUnit
{
    private static int surveyDistance = 2;
    private static int unitSpeed = 2;

    public HexCell destination;
    public bool needsDestination;

    public ParticleSystem.EmissionModule particles;

    public UnitsRoot unitsRoot;
    public BuildingsRoot buildingsRoot;

    public UIUnitInfoOverlay unitInfoOverlay;

    public static string surveyorName = "Surveyor";
    public static string surveyorDescription = "This unit is critical to identifying mineral deposits for your harvesters to mine.";
    public static int surveyorCost = 100;
    public static float surveyorBuildTime = 5.0f;
    public float surveyorBuildProgress;

    public static bool isUnlocked;

    public BuildingSurveyBeacon beaconPrefab;
    public BuildingHarmonicResonator resonatorPrefab;

    public override float ProductionTime { get => surveyorBuildTime; set => surveyorBuildTime = value; }
    public override bool IsUnlocked { get => isUnlocked; set => isUnlocked = value; }

    public override string UnitName { get => surveyorName; set => surveyorName = value; }
    public override string UnitDescription { get => surveyorDescription; set => surveyorDescription = value; }
    public override int UnitCost { get => surveyorCost; set => surveyorCost = value; }
    public override float UnitBuildTime { get => surveyorBuildTime; set => surveyorBuildTime = value; }

    public static int SurveyDistance
    {
        get
        {
            return surveyDistance;
        }
        set
        {
            surveyDistance = value;
        }
    }

    public override float UnitSpeed
    {
        get
        {
            return unitSpeed;
        }
        set
        {
            unitSpeed = (int)value;
        }
    }

    private void Awake()
    {
        unitsRoot = GameObject.FindGameObjectWithTag("unitsroot").GetComponent<UnitsRoot>();
        buildingsRoot = GameObject.FindGameObjectWithTag("buildingsroot").GetComponent<BuildingsRoot>();
        unitInfoOverlay = GameObject.FindGameObjectWithTag("unitcanvas").GetComponent<UIUnitInfoOverlay>();
    }

    public void BuildAtResource(BuildingBaseClass building, HexCell resourceLocation)
    {
        IsBusy = true;
        if (resourceLocation.IsNeighbor(Location))
        {
            StartCoroutine(PlaceBeacon(building, resourceLocation));
        }
        else
        {
            List<HexCell> pathToBuildLocation;
            Grid.FindPath(Location, resourceLocation, (int)UnitSpeed, false);
            pathToBuildLocation = Grid.GetPath();
            pathToBuildLocation.RemoveAt(pathToBuildLocation.Count - 1);
            TravelToResource(pathToBuildLocation, UnitSpeed, building, resourceLocation);
        }
    }

    public IEnumerator ConstructResourceBuilding(BuildingBaseClass building, HexCell resourceLocation)
    {
        //yield return LookAt(resourceLocation.Position);

        Vector3 fullScale = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 zeroScale = new Vector3(0.0f, 0.0f, 0.0f);

        Quaternion fromRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        Quaternion toRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        building.transform.localScale = zeroScale;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime)
        {
            building.transform.localPosition = Vector3.Lerp(this.transform.position, resourceLocation.Position, t);
            building.transform.localScale = Vector3.Lerp(zeroScale, fullScale, t);
            building.transform.localRotation = Quaternion.Lerp(fromRotation, toRotation, t);
            yield return null;
        }

        building.transform.localScale = fullScale;
        building.transform.localRotation = toRotation;

        Grid.MakeChildOfColumn(building.transform, resourceLocation.ColumnIndex);

        building.Location = resourceLocation;

        if (building.GetComponent<BuildingHarmonicResonator>())
        {
            resourceLocation.resonator = building.GetComponent<BuildingHarmonicResonator>();
        }

        if (building.GetComponent<BuildingSurveyBeacon>())
        {
            resourceLocation.beacon = building.GetComponent<BuildingSurveyBeacon>();
            ResourcesRoot.resourcesGlobalList.Add(resourceLocation.Resource);
            building.GetComponent<BuildingSurveyBeacon>().BuildActions();
        }
    }

    public void TravelToResource(List<HexCell> path, float speed, BuildingBaseClass building, HexCell resourceLocation)
    {
        location.Unit = null;
        location = path[path.Count - 1];
        location.Unit = this;
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPathToResource(speed, building, resourceLocation));
    }

    protected IEnumerator TravelPathToResource(float speed, BuildingBaseClass building, HexCell resourceLocation)
    {
        Vector3 a, b, c = pathToTravel[0].Position;
        yield return LookAt(pathToTravel[1].Position);
        if (!currentTravelLocation)
        {
            currentTravelLocation = pathToTravel[0];
        }
        Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
        int currentColumn = currentTravelLocation.ColumnIndex;

        float t = Time.deltaTime * speed;
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

            for (; t < 1.0f; t += Time.deltaTime * speed)
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
        for (; t < 1.0f; t += Time.deltaTime * speed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0.0f;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        transform.localPosition = location.Position;

        yield return PlaceBeacon(building, resourceLocation);

        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;

    }

    public IEnumerator PlaceBeacon(BuildingBaseClass building, HexCell resourceLocation)
    {
        yield return LookAt(resourceLocation.Position);
        unitInfoOverlay.CreateSlider(this.gameObject);

        float inProgress = 0.0f;
        surveyorBuildProgress = 0.0f;

        while (inProgress < surveyorBuildTime)
        {
            inProgress += Time.deltaTime;
            surveyorBuildProgress = inProgress / surveyorBuildTime;
            unitInfoOverlay.UpdateStatus(this.gameObject, surveyorBuildProgress);
            yield return null;
        }

        unitInfoOverlay.DestroySlider(this.gameObject);
        StartCoroutine(ConstructResourceBuilding(building, resourceLocation));
        IsBusy = false;
    }
}
