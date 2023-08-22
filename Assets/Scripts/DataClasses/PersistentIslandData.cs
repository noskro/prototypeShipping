using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PersistentIslandData
{
    public IslandPrefab prefab;
    public bool IslandDiscovered;

    //public List<CityDataSO> CityDataSOList;
    public List<PersistentCityData> PersistentCityDataList;

    public PersistentIslandData(IslandPrefab islandPrefab)
    {
        this.prefab = islandPrefab;

        IslandDiscovered = false;

        PersistentCityDataList = new List<PersistentCityData>();

        foreach (CityDataSO cityDataSo in islandPrefab.cityDataList)
        {
            PersistentCityData persistentCityData = new PersistentCityData(cityDataSo);
            PersistentCityDataList.Add(persistentCityData);

            DemoController.Instance.storyTextManager.ActiveStoryTextEventList.AddRange(cityDataSo.ImmediateStoryTextEvents);
        }
    }

    internal void Reset()
    {
        foreach (PersistentCityData cityData in PersistentCityDataList)
        {
            cityData.ResetRound();
        }
    }
}