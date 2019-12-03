using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProductsRoot : MonoBehaviour
{
    public static List<ProductsList.Product> inProduction = new List<ProductsList.Product>();

    private void Start()
    {
        ProductsList.availableProducts.Add(ProductsList.healingCrystals);
        ProductsList.availableProducts.Add(ProductsList.paperweights);
    }

    public void ProduceProduct(ProductsList.Product product)
    {
        inProduction.Add(product);
        ProductInProduction pip = gameObject.AddComponent<ProductInProduction>();
        pip.product = product;
    }

    public void StopProduction(ProductsList.Product product)
    {
        ProductInProduction[] pips = gameObject.GetComponents<ProductInProduction>();

        foreach (ProductInProduction pip in pips)
        {
            if (pip.product.productTitle == product.productTitle)
            {
                inProduction.Remove(product);
                Destroy(pip);
                break;
            }
        }
    }

    public float GetProductProductionStatus(string productName)
    {
        ProductInProduction[] pips = gameObject.GetComponents<ProductInProduction>();

        foreach (ProductInProduction pip in pips)
        {
            if (pip.product.productTitle == productName)
            {
                return pip.productionPercentage;
            }
        }

        return 0.0f;
    }
}
