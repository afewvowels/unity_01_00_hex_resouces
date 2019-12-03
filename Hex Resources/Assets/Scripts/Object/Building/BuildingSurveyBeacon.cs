using UnityEngine;
using System.Collections;

public class BuildingSurveyBeacon : BuildingBaseClass
{
    public static int surveyBonus = 15;

    public static bool isAvailable;

    public override bool IsAvailable { get => isAvailable; set => isAvailable = value; }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuildActions()
    {
        TechTree.Unlocks.UnlockHarvester();
        TechTree.Unlocks.UnlockProcessing();
        Economy.Dollars += surveyBonus;
    }
}
