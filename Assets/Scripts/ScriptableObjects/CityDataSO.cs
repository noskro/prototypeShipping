﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CityDataSO", order = 3)]
public class CityDataSO: ScriptableObject
{
    public string CityName;

    public Sprite spriteCityLocation;

    public int startingCityLevel;

    public List<StoryTextEventSO> ImmediateStoryTextEvents;
    public List<StoryTextEventSO> TavernStoryTextEvents;
}