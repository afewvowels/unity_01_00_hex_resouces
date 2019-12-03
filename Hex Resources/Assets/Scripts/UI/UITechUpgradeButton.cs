using System;
using UnityEngine;
using UnityEngine.UI;

public class UITechUpgradeButton : MonoBehaviour
{
    public Text buttonText;
    public UITechMenu techMenu;
    public TechTree.Item upgrade;
    public ResearchItemInProgress researchItem;

    public Image buttonImageOverlay;

    public bool isInQueue;

    public Slider progressBar;

    private void Start()
    {
        //progressBar = gameObject.GetComponentInChildren<Slider>();
        //if (isInQueue)
        //{
        //    //progressBar.gameObject.SetActive(false);
        //    buttonImageOverlay.gameObject.SetActive(true);
        //}
    }

    private void FixedUpdate()
    {
        if (isInQueue)
        {
            //buttonImageOverlay.
            //progressBar.value = researchItem.productionProgress;
            //if (progressBar.value > 0.99f)
            //{
            //    Destroy(gameObject);
            //}
            buttonImageOverlay.fillAmount = 1.0f - researchItem.productionProgress;
            if (buttonImageOverlay.fillAmount < 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void DoAction()
    {
        techMenu.upgradeValues = upgrade;
        techMenu.TurnOnUpgradeItems(upgrade);
    }

    public void InitializeTechUpgradeButton()
    {

        if (isInQueue)
        {
            buttonImageOverlay.gameObject.SetActive(true);
            //progressBar = gameObject.GetComponentInChildren<Slider>();
            //progressBar.gameObject.SetActive(true);
        }
    }
}
