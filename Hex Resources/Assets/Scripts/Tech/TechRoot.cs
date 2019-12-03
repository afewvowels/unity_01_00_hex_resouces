using UnityEngine;
using System.Collections;

public class TechRoot : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (gameObject.GetComponents<ResearchItemInProgress>().Length > 0 && gameObject.GetComponents<ResearchItemInProgress>()[0].isResearching == false)
        {
            gameObject.GetComponents<ResearchItemInProgress>()[0].StartResearchItem();
        }
    }
}
