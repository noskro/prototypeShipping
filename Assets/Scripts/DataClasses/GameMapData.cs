
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMapData
{
    public EnumFogOfWar fow = EnumFogOfWar.Undiscovered;

    public PersistentIslandData islandData;
    public int cityIndex;

    public CityLabelView cityLabelOverlay;

    public PathfindingTileNode pathfingingTileNode;

    public GameMapData(Vector2Int cell)
    {
        this.pathfingingTileNode = new PathfindingTileNode();
        this.pathfingingTileNode.setHexPosition(cell);
    }

    internal void Reset(float distanceToStartingCell)
    {
        fow = EnumFogOfWar.Undiscovered; // remove Cartography for now

        if (islandData != null)
        {
            islandData.Reset(); // hasVillageTraded = false;
            islandData = null;
            cityIndex = -1;
        }

        cityLabelOverlay = null;

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

    internal void setCityNameOverlay(CityLabelView cityLabelView)
    {
        this.cityLabelOverlay = cityLabelView;
    }

    internal void SetCityDiscovered(bool v)
    {
        this.CityData.CityDiscovered = v;
        this.cityLabelOverlay.SetCityDiscovered(v);
    }

    internal void SetPathfindingNodeMoveable(bool moveable)
    {
        if (this.pathfingingTileNode != null)
        {
            this.pathfingingTileNode.isMoveable = moveable;
        }
    }

    internal void SetPathfindingNodeNeighbors(List<PathfindingTileNode> neighbors)
    {
        if (this.pathfingingTileNode != null)
        {
            this.pathfingingTileNode.adjacentTiles = neighbors;
        }
    }
}