using System;
using UnityEngine;
using UnityEngine.UI;

public class TechTree : MonoBehaviour
{
    public struct Upgrade
    {
        public string upgradeClassName;
        public string upgradeName;
        public Item[] upgradeItems;

        public Upgrade(string uClassName, string uName, Item[] uItems)
        {
            this.upgradeClassName = uClassName;
            this.upgradeName = uName;
            this.upgradeItems = uItems;
        }
    }

    public struct Item
    {
        public string itemName;
        public int value;
        public string description;
        public int cost;
        public bool applied;
        public float upgradeTime;

        public Item(string itemName, int value, string description, int cost, bool applied, float upgradeTime)
        {
            this.itemName = itemName;
            this.value = value;
            this.description = description;
            this.cost = cost;
            this.applied = applied;
            this.upgradeTime = upgradeTime;
        }
    }

    public class Unlocks
    {
        public static void StartNewGame()
        {
            BuildingGarage.isAvailable = true;
            BuildingProcessing.isAvailable = false;
            BuildingFactory.isAvailable = false;
            BuildingHarmonicResonator.isAvailable = false;

            UnitSurveyor.isUnlocked = true;
            UnitHarvester.isUnlocked = false;
            UnitBuilder.isUnlocked = false;
        }


        public static void UnlockGarage()
        {
            BuildingGarage.isAvailable = true;
        }

        public static void UnlockProcessing()
        {
            BuildingProcessing.isAvailable = true;
        }

        public static void UnlockFactory()
        {
            BuildingFactory.isAvailable = true;
        }

        public static void UnlockResonator()
        {
            BuildingHarmonicResonator.isAvailable = true;
        }

        public static void UnlockSurveyor()
        {
            UnitSurveyor.isUnlocked = true;
        }

        public static void UnlockHarvester()
        {
            UnitHarvester.isUnlocked = true;
        }

        public static void UnlockBuilder()
        {
            UnitBuilder.isUnlocked = true;
        }
    }

    public class Units
    {
        public class Harvester
        {
            public static Upgrade[] upgrades =
            {
                new Upgrade(
                    "Harvester",
                    "Speed",
                    new Item[] {
                            new Item(
                                "Speed",
                                4,
                                "This upgrade will make your harvester units move faster",
                                100,
                                false,
                                30.0f
                                ),
                            new Item(
                                "Speed",
                                6,
                                "This upgrade will make your harvesters move even faster than before",
                                200,
                                false,
                                30.0f
                                ),
                            new Item(
                                "Speed",
                                8,
                                "This upgrade will make your harvesters move lightning fase",
                                400,
                                false,
                                30.0f
                                )
                        }
                    ),
                new Upgrade(
                    "Harvester",
                    "Harvest Speed",
                    new Item[]
                    {
                        new Item(
                            "Harvest Speed",
                            6,
                            "This upgrade will make your harvester units move faster",
                            100,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Harvest Speed",
                            8,
                            "This upgrade will make your harvesters gather resources faster",
                            200,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Harvest Speed",
                            12,
                            "This upgrade will make your harvesters gather resources the fastest",
                            400,
                            false,
                            30.0f
                            )
                    }
                    ),
                new Upgrade(
                    "Harvester",
                    "Storage",
                    new Item[]
                    {
                        new Item(
                            "Max Held Resources",
                            80,
                            "This upgrade will allow your harvesters to hold more resources (80)",
                            100,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Max Held Resources",
                            100,
                            "This upgrade will allow your harvesters to hold even more resources (100)",
                            200,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Max Held Resources",
                            150,
                            "This upgrade will allow your harvesters to hold the most resources (150)",
                            400,
                            false,
                            30.0f
                            )
                    })
            };
        }

        public class Builder
        {
            public static Upgrade[] upgrades =
            {
                new Upgrade(
                    "Builder",
                    "Speed",
                    new Item[]
                    {
                        new Item(
                            "Speed",
                            4,
                            "This upgrade will allow your builders to move faster",
                            100,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Speed",
                            8,
                            "This upgrade will allow your builders to move even faster",
                            200,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Speed",
                            10,
                            "This upgrade will allow your builders to move the fastest",
                            400,
                            false,
                            30.0f
                            )
                    }
                ),
                new Upgrade(
                    "Builder",
                    "Build Speed",
                    new Item[]
                    {
                        new Item(
                            "Build Speed",
                            12,
                            "This upgrade will allow your builders to construct buildings faster",
                            100,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Build Speed",
                            15,
                            "This upgrade will allow your builders to construct buildings even faster",
                            200,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Build Speed",
                            18,
                            "This upgrade will allow your builders to construct buildings the fastest",
                            400,
                            false,
                            30.0f
                            )
                    }
                )
            };
        }
    }

    public class Buildings
    {
        public class Factory
        {
            public static Upgrade[] upgrades =
            {
                new Upgrade(
                    "Factory",
                    "Resource Storage",
                    new Item[]
                    {
                        new Item(
                            "Resource Storage",
                            500,
                            "This upgrade will allow your factory to store more resources",
                            100,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Resource Storage",
                            800,
                            "This upgrade will allow your factory to store even more resources",
                            200,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Resource Storage",
                            1200,
                            "This upgrade will allow your factory to store the most resources",
                            400,
                            false,
                            30.0f
                            )
                    }
                ),
                new Upgrade(
                    "Factory",
                    "Harvester Count",
                    new Item[]
                    {
                        new Item(
                            "Harvester Count",
                            2,
                            "This upgrade will allow your factory to support another harvester",
                            100,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Harvester Count",
                            3,
                            "This upgrade will allow your factory to support a third harvester",
                            200,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Harvester Count",
                            4,
                            "This upgrade will allow your factory to support a fourth harvester",
                            400,
                            false,
                            30.0f
                            )
                    })
            };
        }
    }

    public class Resources
    {
        public class Crystal
        {
            public static Upgrade[] upgrades =
            {
                new Upgrade(
                    "Crystal",
                    "Refresh Rate",
                    new Item[]
                    {
                        new Item(
                            "Refresh Rate",
                            3,
                            "This upgrade will allow resources to replenish themselves",
                            100,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Refresh Rate",
                            6,
                            "This upgrade will allow resources to replenish their resources faster",
                            200,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Refresh Rate",
                            9,
                            "This upgrade will allow resources to replenish their resources the fastest",
                            400,
                            false,
                            30.0f
                            )
                    }
                ),
                new Upgrade(
                    "Crystal",
                    "Capacity",
                    new Item[]
                    {
                        new Item(
                            "Capacity",
                            100,
                            "This upgrade will increase the amount of resources you can harvest",
                            100,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Capacity",
                            150,
                            "This upgrade will further increase the amount of resources you can harvest",
                            200,
                            false,
                            30.0f
                            ),
                        new Item(
                            "Capacity",
                            200,
                            "This upgrade will maximize the amount of resources you can harvest",
                            400,
                            false,
                            30.0f
                            )
                    }
                )
            };
            public static int refreshRate;
            public static int maxResources;

            public static string[] upgradeCategories = { "refresh rate", "max resources" };
        }
    }
}
