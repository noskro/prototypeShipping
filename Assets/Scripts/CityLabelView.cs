using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityLabelView: MonoBehaviour
{
    public TextMeshPro textCityName;
    public TextMeshPro textCityLevel;

    public SpriteRenderer CitySprite;
    public List<Sprite> SpriteCityLevelList;

    internal void SetCityData(PersistentCityData persistentCityData)
    {
        textCityName.text = persistentCityData.CityName;
        //textCityLevel.text = persistentCityData.CityLevel.ToString();

        if (persistentCityData.CityLevel >= 0 && persistentCityData.CityLevel < SpriteCityLevelList.Count)
        {
            CitySprite.sprite = SpriteCityLevelList[persistentCityData.CityLevel];
        }
    }
}