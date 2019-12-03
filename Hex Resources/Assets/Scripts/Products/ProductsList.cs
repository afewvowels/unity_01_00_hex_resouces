using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProductsList : MonoBehaviour
{
    public static List<Product> availableProducts = new List<Product>();

    public struct Product
    {
        public string productTitle;
        public string productDescription;
        public int resource1Cost;
        public int resource2Cost;
        public int resource3Cost;
        public float timeToProduce;
        public int productProfit;

        public Product(string title, string description, int r1cost, int r2cost, int r3cost, float time, int profit)
        {
            this.productTitle = title;
            this.productDescription = description;
            this.resource1Cost = r1cost;
            this.resource2Cost = r2cost;
            this.resource3Cost = r3cost;
            this.timeToProduce = time;
            this.productProfit = profit;
        }
    }

    public static Product healingCrystals = new Product(
        "Healing Crystals",
        "These crystals are good for soothing swollen knees.",
        5,
        0,
        0,
        5.0f,
        2
        );

    public static Product paperweights = new Product(
        "Paperweights",
        "These are paperweights and will look nice on your desk.",
        4,
        0,
        0,
        5.0f,
        1
        );
}
