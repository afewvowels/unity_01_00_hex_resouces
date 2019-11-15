using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUnitsMenu : MonoBehaviour
{
    public RectTransform listContent;

    public UIUnitsIconItem unitIconPrefab;

    public HexFeatureCollection unitsCollection;

    public UnitsRoot unitsRoot;

    public void Open()
    {
        gameObject.SetActive(true);
        FillList();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SelectMenuItem(HexUnit unit)
    {
        unitsRoot.CreateUnit(unit);
        Close();
    }

    public void FillList()
    {
        for (int i = 0; i < listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < unitsCollection.prefabs.Length; i++)
        {
            UIUnitsIconItem menuIcon = Instantiate(unitIconPrefab);
            menuIcon.unitsMenu = this;
            menuIcon.SetFields(unitsCollection.prefabs[i].GetComponent<HexUnit>());
            menuIcon.transform.SetParent(listContent, false);
        }
    }
}
