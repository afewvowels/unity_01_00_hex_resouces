using UnityEngine;

public class ResourceMinerals : ResourceBaseClass
{
    public static int resourceID = 1;

    public GameObject[] crystals;
    public GameObject[] depletedCrystals;

    protected override void ResizeResourceBasedOnAmount()
    {
        if (ResourceAmount >= maxResourceAmount)
        {
            for (int i = 0; i < 5; i++)
            {
                crystals[i].SetActive(true);
                depletedCrystals[i].SetActive(false);
            }
        }
        else if (ResourceAmount > maxResourceAmount * 0.8f)
        {
            for (int i = 0; i < 4; i++)
            {
                crystals[i].SetActive(true);
                depletedCrystals[i].SetActive(false);
            }

            for (int i = 4; i >= 4; i--)
            {
                crystals[i].SetActive(false);
                depletedCrystals[i].SetActive(true);
            }
        }
        else if (ResourceAmount > maxResourceAmount * 0.6f)
        {
            for (int i = 0; i < 3; i++)
            {
                crystals[i].SetActive(true);
                depletedCrystals[i].SetActive(false);
            }

            for (int i = 4; i >= 3; i--)
            {
                crystals[i].SetActive(false);
                depletedCrystals[i].SetActive(true);
            }
        }
        else if (ResourceAmount > maxResourceAmount * 0.4f)
        {
            for (int i = 0; i < 2; i++)
            {
                crystals[i].SetActive(true);
                depletedCrystals[i].SetActive(false);
            }

            for (int i = 4; i >= 2; i--)
            {
                crystals[i].SetActive(false);
                depletedCrystals[i].SetActive(true);
            }
        }
        else if (ResourceAmount > maxResourceAmount * 0.2f)
        {
            for (int i = 0; i < 1; i++)
            {
                crystals[i].SetActive(true);
                depletedCrystals[i].SetActive(false);
            }

            for (int i = 4; i >= 1; i--)
            {
                crystals[i].SetActive(false);
                depletedCrystals[i].SetActive(true);
            }
        }
        else if (ResourceAmount == 0)
        {
            for (int i = 4; i >= 0; i--)
            {
                crystals[i].SetActive(false);
                depletedCrystals[i].SetActive(true);
            }
        }
    }
}
