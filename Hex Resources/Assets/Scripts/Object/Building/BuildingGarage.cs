using UnityEngine;
using System.Collections;

public class BuildingGarage : BuildingBaseClass
{
    public static bool isAvailable = true;

    public UnitHarvester harvesterPrefab;

    public HexCell buildDestination;

    public UIGarageMenu garageMenu;

    public UIUnitInfoOverlay infoOverlay;

    public static float buildTime = 5.0f;

    public static float BuildTime
    {
        get
        {
            return buildTime;
        }
        set
        {
            buildTime = value;
        }
    }

    public override bool IsAvailable { get => isAvailable; set => isAvailable = value; }

    private void Awake()
    {
        TechTree.Unlocks.UnlockSurveyor();
        infoOverlay = GameObject.FindGameObjectWithTag("unitcanvas").GetComponent<UIUnitInfoOverlay>();
        IsBusy = true;
    }

    private void FixedUpdate()
    {
        if (gameObject.GetComponents<UIGarageUnitInProduction>().Length > 0 && gameObject.GetComponents<UIGarageUnitInProduction>()[0].inProduciton == false)
        {
            gameObject.GetComponents<UIGarageUnitInProduction>()[0].StartProduction();
        }
    }

    public override void StartBuildMe()
    {

    }

    public override IEnumerator BuildMe(Color[] colorsArr)
    {
        Material[] materialsArr = GetComponentInChildren<MeshRenderer>().materials;

        Color validBuildColor = new Color(0.75f, 0.75f, 0.75f);

        for (int i = 0; i < materialsArr.Length; i++)
        {
            materialsArr[i].color = validBuildColor;
        }

        infoOverlay.CreateSlider(this.gameObject);
        for (float t = 0.0f; t < buildTime; t += Time.deltaTime)
        {
            infoOverlay.UpdateStatus(this.gameObject.gameObject, t / buildTime);
            yield return null;
        }

        infoOverlay.DestroySlider(this.gameObject);
        IsBusy = false;

        for (int i = 0; i < materialsArr.Length; i++)
        {
            materialsArr[i].color = colorsArr[i];
        }
    }
}
