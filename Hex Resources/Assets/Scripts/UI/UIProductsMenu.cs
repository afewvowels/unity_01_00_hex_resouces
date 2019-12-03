using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIProductsMenu : MonoBehaviour
{
    public HexGameUI hexGameUI;

    public ProductsRoot productsRoot;

    public Text productTitleText;
    public Text productDescriptionText;
    public Text productResource1Text;
    public Text productResource1DynamicText;
    public Text productResource2Text;
    public Text productResource2DynamicText;
    public Text productResource3Text;
    public Text productResource3DynamicText;
    public Text productTimeText;
    public Text productTimeDynamicText;
    public Text productProfitText;
    public Text productProfitDynamicText;

    public RectTransform productsList;
    public RectTransform inProductionList;

    public GameObject produceButton;
    public GameObject cancelProduceButton;

    public UIProductIconItem productIconPrefab;

    public ProductsList.Product activeProduct;

    public GameObject alertPanel;

    private void FixedUpdate()
    {
        if (inProductionList.childCount > 0)
        {
            for (int i = 0; i < inProductionList.childCount; i++)
            {
                UIProductIconItem item = inProductionList.GetChild(i).GetComponent<UIProductIconItem>();
                //item.GetProgressBar().value = productsRoot.GetProductProductionStatus(item.storedProduct.productTitle);
                item.imageOverlay.fillAmount = 1.0f - productsRoot.GetProductProductionStatus(item.storedProduct.productTitle);
            }
        }
    }

    public void Open()
    {
        HexGameUI.menuOpen = true;
        HexMapCamera.Locked = true;
        gameObject.SetActive(true);
        ClearDescriptionText();
        ClearLists();
        InitializeAvailableProductsList();
        InitializeInProductionList();
    }

    public void Close()
    {
        hexGameUI.TurnOffNeighboringHighlights();
        HexGameUI.menuOpen = false;
        ClearLists();
        HexMapCamera.Locked = false;
        gameObject.SetActive(false);
    }

    public void OpenAlert()
    {
        alertPanel.SetActive(true);
    }

    public void CloseAlert()
    {
        alertPanel.SetActive(false);
    }

    private void ClearDescriptionText()
    {
        productTitleText.gameObject.SetActive(false);
        productDescriptionText.gameObject.SetActive(false);
        productResource1Text.gameObject.SetActive(false);
        productResource1DynamicText.gameObject.SetActive(false);
        productResource2Text.gameObject.SetActive(false);
        productResource2DynamicText.gameObject.SetActive(false);
        productResource3Text.gameObject.SetActive(false);
        productResource3DynamicText.gameObject.SetActive(false);
        productTimeText.gameObject.SetActive(false);
        productTimeDynamicText.gameObject.SetActive(false);
        productProfitText.gameObject.SetActive(false);
        productProfitDynamicText.gameObject.SetActive(false);
        produceButton.SetActive(false);
        cancelProduceButton.SetActive(false);
    }

    private void SetDescriptionText(ProductsList.Product product)
    {
        productTitleText.gameObject.SetActive(true);
        productDescriptionText.gameObject.SetActive(true);
        productResource1Text.gameObject.SetActive(true);
        productResource1DynamicText.gameObject.SetActive(true);
        productResource2Text.gameObject.SetActive(true);
        productResource2DynamicText.gameObject.SetActive(true);
        productResource3Text.gameObject.SetActive(true);
        productResource3DynamicText.gameObject.SetActive(true);
        productTimeText.gameObject.SetActive(true);
        productTimeDynamicText.gameObject.SetActive(true);
        productProfitText.gameObject.SetActive(true);
        productProfitDynamicText.gameObject.SetActive(true);
        produceButton.SetActive(true);

        productTitleText.text = product.productTitle;
        productDescriptionText.text = product.productDescription;
        productResource1DynamicText.text = product.resource1Cost.ToString();
        productResource2DynamicText.text = product.resource2Cost.ToString();
        productResource3DynamicText.text = product.resource3Cost.ToString();
        productTimeDynamicText.text = Mathf.RoundToInt(product.timeToProduce).ToString();
        productProfitDynamicText.text = product.productProfit.ToString();
    }

    public void SetActiveProduct(ProductsList.Product product)
    {
        SetDescriptionText(product);
        activeProduct = product;
    }

    public void SetCancelProduct(ProductsList.Product product)
    {
        produceButton.SetActive(false);
        cancelProduceButton.SetActive(true);
    }

    private void InitializeAvailableProductsList()
    {
        for (int i = 0; i < ProductsList.availableProducts.Count; i++)
        {
            UIProductIconItem icon = Instantiate(productIconPrefab);
            icon.productsMenu = this;
            icon.InitializeProductButton(ProductsList.availableProducts[i], false);
            icon.transform.SetParent(productsList.transform, false);
        }
    }

    private void InitializeInProductionList()
    {
        for (int i = 0; i < ProductsRoot.inProduction.Count; i++)
        {
            UIProductIconItem icon = Instantiate(productIconPrefab);
            icon.productsMenu = this;
            icon.InitializeProductButton(ProductsRoot.inProduction[i], true);
            icon.transform.SetParent(inProductionList.transform, false);
        }
    }

    private void ClearLists()
    {
        ClearAvailableProductsList();
        ClearInProductionList();
    }

    private void ClearAvailableProductsList()
    {
        for (int i = 0; i < productsList.childCount; i++)
        {
            Destroy(productsList.GetChild(i).gameObject);
        }
    }

    private void ClearInProductionList()
    {
        for (int i = 0; i < inProductionList.childCount; i++)
        {
            Destroy(inProductionList.GetChild(i).gameObject);
        }
    }

    public void BeginProduction()
    {
        if (ProductsRoot.inProduction.Contains(activeProduct))
        {
            OpenAlert();
        }
        else
        {
            productsRoot.ProduceProduct(activeProduct);
            ClearInProductionList();
            InitializeInProductionList();
        }
    }

    public void CancelProduce()
    {
        if (!ProductsRoot.inProduction.Contains(activeProduct))
        {
            OpenAlert();
        }
        else
        {
            productsRoot.StopProduction(activeProduct);
            ClearInProductionList();
            InitializeInProductionList();
        }
    }
}
