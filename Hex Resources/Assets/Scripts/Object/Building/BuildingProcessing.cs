using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingProcessing : BuildingBaseClass
{
    public HexCell dropoffCell;

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

    public static bool isAvailable;

    public override bool IsAvailable { get => isAvailable; set => isAvailable = value; }

    private void Awake()
    {
        infoOverlay = GameObject.FindGameObjectWithTag("unitcanvas").GetComponent<UIUnitInfoOverlay>();
        IsBusy = true;
    }



    public override IEnumerator BuildMe(Color[] colorsArr)
    {
        List<Material> materialsArr = new List<Material>();

        for (int i = 0; i < GetComponentsInChildren<MeshRenderer>().Length; i++)
        {
            foreach (Material material in GetComponentsInChildren<MeshRenderer>()[i].materials)
            {
                materialsArr.Add(material);
            }
        }

        Color validBuildColor = new Color(0.75f, 0.75f, 0.75f);

        for (int i = 0; i < materialsArr.Count; i++)
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

        for (int i = 0; i < materialsArr.Count; i++)
        {
            materialsArr[i].color = colorsArr[i];
        }
    }
}
