using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

public class UnitHarvester : HexUnit
{
    [SerializeField]
    private int heldResources;
    private static int maxHeldResources = 50;
    private static int harvestAmount = 2;
    private static int unitSpeed = 2;

    public ResourcesRoot resourcesRoot;
    public BuildingsRoot buildingsRoot;

    public HexCell destination;
    public HexCell processingBuildingLocation;

    public bool needsDestination;

    ParticleSystem.EmissionModule particles;

    public GameObject resourcesInBedLow;
    public GameObject resourcesInBedMedium;
    public GameObject resourcesInBedHigh;
    public GameObject bucketRoot;

    public Animator animator;

    public static string harvesterName = "Harvester";
    public static string harvesterDescription = "This unit gathers resources and returns them to your processing plant.";
    public static int harvesterCost = 80;
    public static float harvesterBuildTime = 5.0f;

    public static bool isUnlocked;

    public override float ProductionTime { get => harvesterBuildTime; set => harvesterBuildTime = value; }
    public override bool IsUnlocked { get => isUnlocked; set => isUnlocked = value; }

    public override string UnitName { get => harvesterName; set => harvesterName = value; }
    public override string UnitDescription { get => harvesterDescription; set => harvesterDescription = value; }
    public override int UnitCost { get => harvesterCost; set => harvesterCost = value; }
    public override float UnitBuildTime { get => harvesterBuildTime; set => harvesterBuildTime = value; }

    public override float UnitSpeed { get => unitSpeed; set => unitSpeed = (int)value; }

    public int HeldResources
    {
        get
        {
            return heldResources;
        }
        set
        {
            heldResources = value;

            if (heldResources > maxHeldResources)
            {
                heldResources = maxHeldResources;
            }
            else if (heldResources < 0)
            {
                heldResources = 0;
            }

            SetResourcesInBucket();
        }
    }

    private void Awake()
    {
        //needsDestination = true;
        TechTree.Unlocks.UnlockProcessing();
        heldResources = 0;
        resourcesRoot = GameObject.FindGameObjectWithTag("resourcesroot").GetComponent<ResourcesRoot>();
        buildingsRoot = GameObject.FindGameObjectWithTag("buildingsroot").GetComponent<BuildingsRoot>();
        particles = GetComponentInChildren<ParticleSystem>().emission;
        particles.rateOverTime = new ParticleSystem.MinMaxCurve(0.0f);
        animator = gameObject.GetComponent<Animator>();
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
                destination = pathToTravel[pathToTravel.Count - 1];
                pathToTravel.RemoveAt(pathToTravel.Count - 1);
            }
            else
            {
                pathToTravel = buildingsRoot.GetNearestBuildingPath(Location);
                foreach (HexCell neighbor in pathToTravel[pathToTravel.Count - 1].GetNeighbors())
                {
                    if (neighbor.Building)
                    {
                        destination = neighbor;
                        break;
                    }
                }
            }

            if (pathToTravel.Count > 1)
            {
                TravelHarvester(pathToTravel);
            }
            else
            {
                destination = IsAdjacentToDestination(Location);
                StartCoroutine(DoAdjacent());
            }
        }
    }

    public override void Travel(List<HexCell> path, float speed = 2.0f)
    {
        location.Unit = null;
        location = path[path.Count - 1];
        location.Unit = this;
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath(unitSpeed));
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
        particles.rateOverTime = new ParticleSystem.MinMaxCurve(35.0f);
        Vector3 a, b, c = pathToTravel[0].Position;
        yield return LookAt(pathToTravel[1].Position);
        if (!currentTravelLocation)
        {
            currentTravelLocation = pathToTravel[0];
        }
        Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
        int currentColumn = currentTravelLocation.ColumnIndex;

        float t = Time.deltaTime * unitSpeed;
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

            for (; t < 1.0f; t += Time.deltaTime * unitSpeed)
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
        for (; t < 1.0f; t += Time.deltaTime * unitSpeed)
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

        particles.rateOverTime = new ParticleSystem.MinMaxCurve(0.0f);
        yield return DoAdjacent();
    }

    private IEnumerator DoAdjacent()
    {
        yield return LookAt(destination.Position);

        if (destination.Resource)
        {
            for (float t = 0.0f; t < 0.35f; t += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(Location.transform.position, destination.transform.position, t);
                yield return null;
            }
            animator.SetBool("isMining", true);
            yield return new WaitForSeconds(0.1f);
            destination.Resource.StartHarvest();
            while (heldResources < maxHeldResources && destination.Resource.ResourceAmount != 0)
            {
                destination.Resource.ResourceAmount -= harvestAmount;
                HeldResources += harvestAmount;
                yield return new WaitForSeconds(0.5f);
            }
            //StartCoroutine(destination.Resource.EndHarvest());
            animator.SetBool("isMining", false);
            for (float t = 0.35f; t > 0.0f; t -= Time.deltaTime)
            {
                transform.position = Vector3.Lerp(Location.transform.position, destination.transform.position, t);
                yield return null;
            }
            destination.Resource.EndHarvest();
        }

        else if (destination.Building)
        {
            for (float t = 0.0f; t < 0.6f; t += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(Location.transform.position, destination.transform.position, t);
                yield return null;
            }

            yield return LookAt(Location.transform.position);

            Quaternion fromRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
            Quaternion toRotation = Quaternion.Euler(-140.0f, 0.0f, 0.0f);

            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * 2.0f)
            {
                bucketRoot.transform.localRotation = Quaternion.Lerp(fromRotation, toRotation, t);
                yield return null;
            }

            while (heldResources > 0)
            {
                HeldResources -= harvestAmount;
                //destination.Building.ResourceAmount += harvestAmount;
                Economy.Crystals += harvestAmount;
                yield return new WaitForSeconds(0.1f);
            }

            for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime * 2.0f)
            {
                bucketRoot.transform.localRotation = Quaternion.Lerp(fromRotation, toRotation, t);
                yield return null;
            }

            for (float t = 0.6f; t > 0.0f; t -= Time.deltaTime)
            {
                transform.position = Vector3.Lerp(Location.transform.position, destination.transform.position, t);
                yield return null;
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
            if (neighbor.Resource && HeldResources < maxHeldResources)
            {
                return neighbor;
            }
            else if (neighbor.Building && HeldResources > 0)
            {
                return neighbor;
            }
        }

        return null;
    }

    public static void DoUpgrade(string propName, int value)
    {
        bool isSet;

        switch (propName.ToLower())
        {
            case "speed":
                unitSpeed = value;
                isSet = true;
                break;
            case "harvest speed":
                harvestAmount = value;
                isSet = true;
                break;
            case "max held resources":
                maxHeldResources = value;
                isSet = true;
                break;
            default:
                Debug.Log("Bad upgrade propName supplied");
                isSet = false;
                break;
        }

        if (isSet)
        {
            SetUpgradeActive(propName, value);
        }
        else
        {
            Debug.Log("error setting upgrade to applied");
        }
    }

    private static void SetUpgradeActive(string propName, int _value)
    {
        for (int i = 0; i < TechTree.Units.Harvester.upgrades.Length; i++)
        {
            for (int j = 0; j < TechTree.Units.Harvester.upgrades[i].upgradeItems.Length; j++)
            {
                if (TechTree.Units.Harvester.upgrades[i].upgradeItems[j].itemName.ToLower() == propName.ToLower() &&
                    TechTree.Units.Harvester.upgrades[i].upgradeItems[j].value == _value)
                {
                    TechTree.Units.Harvester.upgrades[i].upgradeItems[j].applied = true;
                    break;
                }
            }
        }
    }

    private void SetResourcesInBucket()
    {
        if (HeldResources == maxHeldResources)
        {
            resourcesInBedHigh.SetActive(true);
            resourcesInBedMedium.SetActive(false);
            resourcesInBedLow.SetActive(false);
        }
        else if (HeldResources > maxHeldResources * 0.66f)
        {
            resourcesInBedHigh.SetActive(false);
            resourcesInBedMedium.SetActive(true);
            resourcesInBedLow.SetActive(false);
        }
        else if (HeldResources > maxHeldResources * 0.33f)
        {
            resourcesInBedHigh.SetActive(false);
            resourcesInBedMedium.SetActive(false);
            resourcesInBedLow.SetActive(true);
        }
        else
        {
            resourcesInBedHigh.SetActive(false);
            resourcesInBedMedium.SetActive(false);
            resourcesInBedLow.SetActive(false);
        }
    }

    public override IEnumerator SpawnUnitFromGarage(HexCell from, HexCell to)
    {
        needsDestination = false;
        Vector3 endScale = transform.localScale;
        Vector3 beginScale = new Vector3(0.0f, 0.0f, 0.0f);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(from.Position, to.Position, t);
            transform.localScale = Vector3.Lerp(beginScale, endScale, t);
            yield return null;
        }
        needsDestination = true;
    }
}
