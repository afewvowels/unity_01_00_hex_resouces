using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIProductIconItem : MonoBehaviour
{
    public Text buttonText;
    public ProductsList.Product storedProduct;

    public UIProductsMenu productsMenu;

    public Slider productionProgress;

    public Image imageOverlay;

    public bool isInProduction;

    public void InitializeProductButton(ProductsList.Product product, bool isInProduction)
    {
        //productionProgress = gameObject.GetComponentInChildren<Slider>();
        this.isInProduction = isInProduction;
        storedProduct = product;
        buttonText.text = storedProduct.productTitle;
        if (isInProduction)
        {
            imageOverlay.gameObject.SetActive(true);
        }
        //if (isInProduction)
        //{
        //    productionProgress.gameObject.SetActive(true);
        //}
        //else
        //{
        //    productionProgress.gameObject.SetActive(false);
        //}
    }

    public void DoAction()
    {
        if (isInProduction)
        {
            productsMenu.SetCancelProduct(storedProduct);
        }
        else
        {
            productsMenu.SetActiveProduct(storedProduct);
        }
    }

    //public Slider GetProgressBar()
    //{
    //    return productionProgress;
    //}
}
