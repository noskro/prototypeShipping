﻿using System;
using UnityEngine;

public class PersistentCityData
{
    public string CityName;
    public int CityLevel;

    public Sprite sprite;

    public int TimesTraded;
    public bool HasTradedThisRun;

    public bool DocksBuild;
    public bool TavernBuild;
    public bool IndustryBuild;
    public bool TraderBuild;
    public bool BeaconBuild;

    public bool CityDiscovered;

    public PersistentCityData(CityDataSO cityDataSo)
    {
        CityName = cityDataSo.CityName;
        CityLevel = cityDataSo.startingCityLevel;

        sprite = cityDataSo.spriteCityLocation;

        DocksBuild = false;
        TavernBuild = false;
        IndustryBuild = false;
        TraderBuild = false;
        BeaconBuild = false;

        CityDiscovered = false;
    }

    internal void ResetRound()
    {
        HasTradedThisRun = false;
    }

    internal void SetTradedThisRun(bool value)
    {
        HasTradedThisRun = value;

        if (value)
        {
            TimesTraded++;
        }   
    }
}