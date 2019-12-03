using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingHub : BuildingBaseClass
{
    public List<BuildingBaseClass> hubAddOns;

    public BuildingGarage buildingGarage;
    public BuildingProcessing buildingProcessing;
    public BuildingFactory buildingFactory;

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

    private void Start()
    {
        TechTree.Unlocks.UnlockGarage();
        TechTree.Unlocks.UnlockSurveyor();
        AddBuildingsToList();
        infoOverlay = GameObject.FindGameObjectWithTag("unitcanvas").GetComponent<UIUnitInfoOverlay>();
        IsBusy = true;
    }

    public void AddBuildingsToList()
    {
        if (!hubAddOns.Contains(buildingGarage))
        {
            hubAddOns.Add(buildingGarage);
        }

        if (!hubAddOns.Contains(buildingProcessing))
        {
            hubAddOns.Add(buildingProcessing);
        }

        if (!hubAddOns.Contains(buildingFactory))
        {
            hubAddOns.Add(buildingFactory);
        }
    }

    public void RemoveGarageFromList()
    {
        if (hubAddOns.Contains(buildingGarage))
        {
            hubAddOns.Remove(buildingGarage);
        }
    }

    public void RemoveProcessingFromList()
    {
        if (hubAddOns.Contains(buildingProcessing))
        {
            hubAddOns.Remove(buildingProcessing);
        }
    }

    public void RemoveFactoryFromList()
    {
        if (hubAddOns.Contains(buildingFactory))
        {
            hubAddOns.Remove(buildingFactory);
        }
    }

    public override IEnumerator BuildMe()
    {
        Material[] materialsArr = GetComponentInChildren<MeshRenderer>().materials;
        Color[] colorsArr = new Color[materialsArr.Length];

        Color validBuildColor = new Color(0.75f, 0.75f, 0.75f);

        for (int i = 0; i < materialsArr.Length; i++)
        {
            colorsArr[i] = materialsArr[i].color;
        }

        for (int i = 0; i < materialsArr.Length; i++)
        {
            materialsArr[i].color = validBuildColor;
        }

        infoOverlay.CreateSlider(this.gameObject);
        for (float t = 0.0f; t < buildTime; t += Time.deltaTime)
        {
            infoOverlay.UpdateStatus(this.gameObject, t / buildTime);
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
