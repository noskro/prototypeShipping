
using System;
using UnityEngine;

public class GameMapData
{
    public EnumFogOfWar fow = EnumFogOfWar.Undiscovered;

    public PersistentIslandData islandData;
    public int cityIndex;



    internal void Reset(float distanceToStartingCell)
    {
        fow = EnumFogOfWar.Undiscovered; // remove Cartography for now

        if (islandData != null)
        {
            islandData.Reset(); // hasVillageTraded = false;
        }

        //CartographyLevelSO currentCartographyLevel = DemoController.Instance.GetCurrentCartograhpyLevel();

        //if (distanceToStartingCell == currentCartographyLevel.CartographyRadius)
        //{
        //    if (!fow.Equals(EnumFogOfWar.Undiscovered))
        //    {
        //        fow = EnumFogOfWar.Fog;
        //    }
        //}
        //else if (distanceToStartingCell > currentCartographyLevel.CartographyRadius)
        //{
        //    // cell is too far from center to be recorded by cartograph
        //    fow = EnumFogOfWar.Undiscovered;
        //}

    }

    public PersistentCityData CityData
    {
        get
        {
            if (islandData != null && cityIndex >= 0 && cityIndex < islandData.PersistentCityDataList.Count)
            {
                return islandData.PersistentCityDataList[cityIndex];
            }
            else
            {
                return null;
            }
        }

        set
        {
            if (cityIndex >= 0 && cityIndex < islandData.PersistentCityDataList.Count && value != null)
            {
                islandData.PersistentCityDataList[cityIndex] = value;
            }
        }
    }

    internal void setCityData(PersistentIslandData islandData, int iCityIndex)
    {
        this.islandData = islandData;
        this.cityIndex = iCityIndex;
    }

    internal bool hasVillageTraded()
    {
        return islandData.PersistentCityDataList[cityIndex].HasTradedThisRun;
    }
}