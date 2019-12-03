using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGarageUnitIconItem : MonoBehaviour
{
    public Text buttonText;

    public UIGarageMenu garageMenu;

    public HexUnit buttonUnit;

    public bool isInQueue;

    public Slider progressSlider;

    public Image progressImage;

    public UIGarageUnitInProduction uip;

    private void FixedUpdate()
    {
        if (isInQueue)
        {
            //progressSlider.value = uip.GetProgress();
            //if (progressSlider.value >= 0.99f)
            //{
            //    Destroy(gameObject);
            //}
            progressImage.fillAmount = 1.0f - uip.GetProgress();
            if (progressImage.fillAmount < 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void DoSelect()
    {
        if (!isInQueue)
        {
            garageMenu.SelectUnitToBuild(buttonUnit);
        }
    }

    public void InitializeGarageUnitIconButton(HexUnit unit)
    {
        //progressSlider = gameObject.GetComponentInChildren<Slider>();

        if (isInQueue)
        {
            //progressSlider.gameObject.SetActive(false);
            progressImage.gameObject.SetActive(true);
        }

        buttonUnit = unit;
        if (unit.GetComponent<UnitBuilder>())
        {
            buttonText.text = UnitBuilder.builderName;
        }
        else if (unit.GetComponent<UnitHarvester>())
        {
            buttonText.text = UnitHarvester.harvesterName;
        }
        else
        {
            buttonText.text = unit.unitName;
        }
    }
}
