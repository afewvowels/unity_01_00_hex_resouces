using UnityEngine;
using System.Collections;

public class BuildingFactory : BuildingBaseClass
{
    public UIProductsMenu productsMenu;

    public static bool isAvailable;
    public static float buildTime = 5.0f;

    public UIUnitInfoOverlay infoOverlay;

    public override bool IsAvailable { get => isAvailable; set => isAvailable = value; }
    public static float BuildTime { get; set; }

    private void Start()
    {
        infoOverlay = GameObject.FindGameObjectWithTag("unitcanvas").GetComponent<UIUnitInfoOverlay>();
        IsBusy = true;
        StartCoroutine(BuildMe());
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
