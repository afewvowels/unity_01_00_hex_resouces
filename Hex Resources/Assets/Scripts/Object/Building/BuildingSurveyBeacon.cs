using UnityEngine;
using System.Collections;

public class BuildingSurveyBeacon : BuildingBaseClass
{
    public static int surveyBonus = 15;

    public static bool isAvailable;

    public override bool IsAvailable { get => isAvailable; set => isAvailable = value; }

    public void BuildActions()
    {
        TechTree.Unlocks.UnlockHarvester();
        TechTree.Unlocks.UnlockProcessing();
        Economy.Dollars += surveyBonus;
    }
}
