using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class metaUpgradeCityUI : MonoBehaviour
{
    public TextMeshProUGUI textCityName;
    public TextMeshProUGUI textIslandName;
    public Button buttonDocks;
    public Button buttonTavern;
    public Button buttonIndustry;
    public Button buttonTrader;
    public Button buttonBeacon;

    private PersistentCityData ciytData;

    public void SetCityData(PersistentCityData cityData)
    {
        if (cityData.CityDiscovered) 
        {
            textCityName.text = cityData.CityName;
            textIslandName.text = "Island";

            buttonDocks.enabled = false;
            buttonDocks.GetComponent<Image>().color = Color.green;
            buttonDocks.GetComponentInChildren<TextMeshProUGUI>().text = "Docks";
        }
        else
        {
            textCityName.text = "Unknown";
            textIslandName.text = "Island";

            buttonDocks.gameObject.SetActive(false);
            buttonTavern.gameObject.SetActive(false);
            buttonIndustry.gameObject.SetActive(false);
            buttonTrader.gameObject.SetActive(false);
            buttonBeacon.gameObject.SetActive(false);
        }
    }

    internal void SetCityData(CityDataSO cityData)
    {
        textCityName.text = "Unknown";
        textIslandName.text = "Unknown";

        buttonDocks.gameObject.SetActive(false);
        buttonTavern.gameObject.SetActive(false);
        buttonIndustry.gameObject.SetActive(false);
        buttonTrader.gameObject.SetActive(false);
        buttonBeacon.gameObject.SetActive(false);

    }
}
