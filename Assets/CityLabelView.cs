using System;
using TMPro;
using UnityEngine;

public class CityLabelView: MonoBehaviour
{
    public TextMeshPro textCityName;
    public TextMeshPro textCityLevel;

    internal void SetCityData(PersistentCityData persistentCityData)
    {
        textCityName.text = persistentCityData.CityName;
        //textCityLevel.text = persistentCityData.CityLevel.ToString();
    }
}