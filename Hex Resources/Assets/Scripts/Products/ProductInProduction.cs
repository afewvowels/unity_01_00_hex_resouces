using UnityEngine;
using System.Collections;

public class ProductInProduction : MonoBehaviour
{
    public ProductsList.Product product;

    private float productionTimer;

    public float productionPercentage;

    private void Start()
    {
        ResetProductionTimer();
    }

    private void ResetProductionTimer()
    {
        productionTimer = 0.0f;
        productionPercentage = 0.0f;
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(productionTimer) < 0.01f)
        {
            StartCoroutine(ProduceProduct());
        }
    }

    public void StopProduction()
    {
        StopAllCoroutines();
    }

    private IEnumerator ProduceProduct()
    {
        while (productionTimer < product.timeToProduce)
        {
            productionTimer += Time.deltaTime;
            productionPercentage = productionTimer / product.timeToProduce;
            yield return null;
        }

        if (Economy.Crystals >= product.resource1Cost)
        {
            Economy.Crystals -= product.resource1Cost;
            Economy.Dollars += product.productProfit;
        }

        ResetProductionTimer();
        yield return null;
    }
}
