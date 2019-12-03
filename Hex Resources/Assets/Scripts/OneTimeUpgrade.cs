using UnityEngine;
using System.Collections;

public class OneTimeUpgrade : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (Economy.crystals > 0)
        {
            TechTree.Unlocks.UnlockFactory();
            Destroy(this);
        }
    }
}
