using UnityEngine;
using UnityEngine.UI;

public class Economy : MonoBehaviour
{
    public static Text economyText;

    public static Text crystalsText;

    public static int crystals;

    public static int Crystals
    {
        get
        {
            return crystals;
        }
        set
        {
            crystals = value;
            Economy.UpdateCrystalsText();
        }
    }

    public static int dollars;

    public static int Dollars
    {
        get
        {
            return dollars;
        }
        set
        {
            dollars = value;
            Economy.UpdateDollarsText();
        }
    }

    private void Awake()
    {
        economyText = GameObject.FindGameObjectWithTag("economytext").GetComponent<Text>();
        crystalsText = GameObject.FindGameObjectWithTag("crystalstext").GetComponent<Text>();
    }

    private static void UpdateDollarsText()
    {
        economyText.text = dollars.ToString();
    }

    private static void UpdateCrystalsText()
    {
        crystalsText.text = crystals.ToString();
    }

    public static void StartNewGame()
    {
        Dollars = 250;
        Crystals = 0;
    }
}
