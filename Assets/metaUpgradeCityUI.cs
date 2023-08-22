using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaUpgradeCityUI : MonoBehaviour
{
    public TextMeshProUGUI textCityName;
    public TextMeshProUGUI textIslandName;
    public Image imageCityLocation;
    public Button buttonDocks;
    public Button buttonTavern;
    public Button buttonIndustry;
    public Button buttonTrader;
    public Button buttonBeacon;

    public int priceDocks;
    public int priceTavern;
    public int priceIndustry;
    public int priceTrader;
    public int priceBeacon;

    public Sprite UndiscoveredIsland;

    private PersistentCityData cityData;
    private PersistentIslandData islandData;

    public void SetCityData(PersistentCityData cityData, PersistentIslandData islandData)
    {
        this.cityData = cityData;
        this.islandData = islandData;

        if (cityData.CityDiscovered) 
        {
            textCityName.text = cityData.CityName;
            imageCityLocation.sprite = cityData.cityDataSO.spriteCityLocation;

            if (cityData.DocksBuild)
            {
                buttonDocks.enabled = false;
                buttonDocks.GetComponent<Image>().color = Color.green;
                buttonDocks.GetComponentInChildren<TextMeshProUGUI>().text = "Docks";
            } 
            else
            {
                buttonDocks.enabled = true;
                buttonDocks.GetComponentInChildren<TextMeshProUGUI>().text = "Docks\n(" + priceDocks + "g)";
            }
            if (cityData.TavernBuild)
            {
                buttonTavern.enabled = false;
                buttonTavern.GetComponent<Image>().color = Color.green;
                buttonTavern.GetComponentInChildren<TextMeshProUGUI>().text = "Tavern";
            }
            else
            {
                buttonDocks.enabled = true;
                buttonTavern.GetComponentInChildren<TextMeshProUGUI>().text = "Tavern\n(" + priceTavern + "g)";
            }
            if (cityData.IndustryBuild)
            {
                buttonIndustry.enabled = false;
                buttonIndustry.GetComponent<Image>().color = Color.green;
                buttonIndustry.GetComponentInChildren<TextMeshProUGUI>().text = "Industry";
            }
            else
            {
                buttonDocks.enabled = true;
                buttonIndustry.GetComponentInChildren<TextMeshProUGUI>().text = "Industry\n(" + priceIndustry + "g)";
            }
            if (cityData.TraderBuild)
            {
                buttonTrader.enabled = false;
                buttonTrader.GetComponent<Image>().color = Color.green;
                buttonTrader.GetComponentInChildren<TextMeshProUGUI>().text = "Trader";
            }
            else
            {
                buttonDocks.enabled = true;
                buttonTrader.GetComponentInChildren<TextMeshProUGUI>().text = "Trader\n(" + priceTrader + "g)";
            }
            if (cityData.BeaconBuild)
            {
                buttonBeacon.enabled = false;
                buttonBeacon.GetComponent<Image>().color = Color.green;
                buttonBeacon.GetComponentInChildren<TextMeshProUGUI>().text = "Beacon";
            }
            else
            {
                buttonDocks.enabled = true;
                buttonBeacon.GetComponentInChildren<TextMeshProUGUI>().text = "Beacon\n(" + priceBeacon + "g)";
            }

        }
        else
        {
            textCityName.text = "Unknown";
            imageCityLocation.sprite = cityData.cityDataSO.spriteCityLocation;
            imageCityLocation.color = Color.black;

            buttonDocks.gameObject.SetActive(false);
            buttonTavern.gameObject.SetActive(false);
            buttonIndustry.gameObject.SetActive(false);
            buttonTrader.gameObject.SetActive(false);
            buttonBeacon.gameObject.SetActive(false);
        }

        if (islandData.IslandDiscovered)
        {
            textIslandName.text = islandData.prefab.IslandName;
        }
        else
        {
            textIslandName.text = "Unknown";
            imageCityLocation.sprite = UndiscoveredIsland;
            imageCityLocation.color = Color.white;
        }
    }

    internal void SetCityData(CityDataSO cityData, PersistentIslandData islandData)
    {
        this.islandData = islandData;

        if (islandData.IslandDiscovered)
        {
            textIslandName.text = islandData.prefab.IslandName;
            imageCityLocation.sprite = cityData.spriteCityLocation;
            imageCityLocation.color = Color.black;
        }
        else
        {
            textIslandName.text = "Unknown";
            imageCityLocation.sprite = UndiscoveredIsland;
        }

        textCityName.text = "Unknown";

        buttonDocks.gameObject.SetActive(false);
        buttonTavern.gameObject.SetActive(false);
        buttonIndustry.gameObject.SetActive(false);
        buttonTrader.gameObject.SetActive(false);
        buttonBeacon.gameObject.SetActive(false);
    }

    internal void SetCityData(CityDataSO cityData, IslandPrefab islandData)
    {
        textIslandName.text = "Unknown";
        textCityName.text = "Unknown";

        imageCityLocation.sprite = cityData.spriteCityLocation;
        imageCityLocation.sprite = UndiscoveredIsland;

        buttonDocks.gameObject.SetActive(false);
        buttonTavern.gameObject.SetActive(false);
        buttonIndustry.gameObject.SetActive(false);
        buttonTrader.gameObject.SetActive(false);
        buttonBeacon.gameObject.SetActive(false);
    }

    public void BuildDocks()
    {
        int gold = DemoController.Instance.shipController.shipStats.Gold;

        if (gold >= priceDocks)
        {
            DemoController.Instance.shipController.shipStats.Gold -= priceDocks;
            this.cityData.DocksBuild = true;
            this.SetCityData(this.cityData, this.islandData);
        }
    }

    public void BuildTavern()
    {
        int gold = DemoController.Instance.shipController.shipStats.Gold;

        if (gold >= priceTavern)
        {
            DemoController.Instance.shipController.shipStats.Gold -= priceTavern;
            this.cityData.TavernBuild = true;
            this.SetCityData(this.cityData, this.islandData);
        }
    }

    public void BuildIndustry()
    {
        int gold = DemoController.Instance.shipController.shipStats.Gold;

        if (gold >= priceIndustry)
        {
            DemoController.Instance.shipController.shipStats.Gold -= priceIndustry;
            this.cityData.IndustryBuild = true;
            this.SetCityData(this.cityData, this.islandData);
        }
    }

    public void BuildTrader()
    {
        int gold = DemoController.Instance.shipController.shipStats.Gold;

        if (gold >= priceTrader)
        {
            DemoController.Instance.shipController.shipStats.Gold -= priceTrader;
            this.cityData.TraderBuild = true;
            this.SetCityData(this.cityData, this.islandData);
        }
    }

    public void BuildBeacon()
    {
        int gold = DemoController.Instance.shipController.shipStats.Gold;

        if (gold >= priceBeacon)
        {
            DemoController.Instance.shipController.shipStats.Gold -= priceBeacon;
            this.cityData.BeaconBuild = true;
            this.SetCityData(this.cityData, this.islandData);
        }
    }
}
