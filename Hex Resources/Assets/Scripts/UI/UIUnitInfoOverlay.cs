using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIUnitInfoOverlay : MonoBehaviour
{
    public Camera mainCamera;

    public RectTransform unitCanvas;

    public GameObject statusBarPrefab;

    private Vector3 offset = new Vector3(0.0f, 15.0f, 15.0f);

    public static Dictionary<GameObject, GameObject> healthBarDictionary = new Dictionary<GameObject, GameObject>();

    private void Start()
    {
        //unitCanvas = GameObject.FindGameObjectWithTag("unitcanvas").GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        if (healthBarDictionary.Count > 0)
        {
            PlaceSliderOnUnit();
        }
    }

    public void CreateSlider(GameObject go)
    {
        GameObject statusBar = Instantiate(statusBarPrefab);
        statusBar.transform.SetParent(unitCanvas.transform, false);
        healthBarDictionary.Add(go, statusBar);
        PlaceSliderOnUnit();
    }

    public void DestroySlider(GameObject go)
    {
        Destroy(healthBarDictionary[go].gameObject);
        healthBarDictionary.Remove(go);
    }

    public void PlaceSliderOnUnit()
    {
        foreach (GameObject key in healthBarDictionary.Keys)
        {
            GameObject statusBar = healthBarDictionary[key];
            statusBar.transform.position = mainCamera.WorldToScreenPoint(key.transform.position);
            //Vector3 statusBarPosition = statusBar.transform.position;
            //Vector2 screenPosition = mainCamera.WorldToScreenPoint(key.transform.position);
            //screenPosition += new Vector2(0.0f, 2.0f);
            //RectTransformUtility.ScreenPointToWorldPointInRectangle(unitCanvas, screenPosition, mainCamera, out statusBarPosition);
            //statusBar.transform.position = statusBarPosition;
        }
    }

    public void UpdateStatus(GameObject go, float status)
    {
        healthBarDictionary[go].GetComponentsInChildren<Image>()[1].fillAmount = status;
    }
}
