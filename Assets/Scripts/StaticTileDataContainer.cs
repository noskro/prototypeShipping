using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StaticTileDataContainer : MonoBehaviour
{
    public Tilemap TilemapMap;
    public Tilemap TilemapObjects;
    public Tilemap TilemapFOW;

    public CustomTile CustomTileWater;
    public CustomTile CustomTileCoastalWater;
    public CustomTile CustomTileBlack;
    public CustomTile CustomTileBlackFog;
    public CustomTile CustomTileVillage;

    public Transform CityLabelPrefab;

    public int mapHeight;
    public int mapWidth;

    public GameMapData[,] gameMapData;
    public List<PersistentIslandData> UsedIslands = new List<PersistentIslandData>();

    private static StaticTileDataContainer instance;

    public static StaticTileDataContainer Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateGameMapDataArray();
    }

    public void CreateGameMapDataArray()
    { 
        gameMapData = new GameMapData[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                gameMapData[x, y] = new GameMapData(); //.fow = EnumFogOfWar.Undiscovered;
            }
        }
    }

    internal Vector2Int GetMapStartingCoordinates()
    {
        return new Vector2Int((StaticTileDataContainer.Instance.mapWidth / 2), (StaticTileDataContainer.Instance.mapHeight / 2));
    }

    internal void CheckCityDiscovered()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GameMapData gmd = gameMapData[x, y];
                if (gmd.CityData != null)
                {
                    if (gmd.fow.Equals(EnumFogOfWar.Visible))
                    {
                        gmd.SetCityDiscovered(true);
                    }
                }
            }
        }
    }

    internal bool IsTradedWith(CityDataSO revelantCity)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GameMapData gmd = gameMapData[x, y];

                if (gmd.CityData != null)
                {
                    if (gmd.CityData.CityName.Equals(revelantCity.CityName))
                    {
                        return gmd.CityData.TimesTraded > 0;
                    }
                }
            }
        }

        return false;
    }

    internal void CheckIslandDiscovered()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GameMapData gmd = gameMapData[x, y];
                if (gmd.islandData != null)
                {
                    if (gmd.fow.Equals(EnumFogOfWar.Visible))
                    {
                        gmd.islandData.IslandDiscovered = true;
                    }
                }
            }
        }
    }

    internal bool IsCityDiscovered(CityDataSO value)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GameMapData gmd = gameMapData[x, y];

                if (gmd == null)
                {
                    gmd = new GameMapData();
                }

                if (gmd.CityData != null)
                {
                    if (gmd.CityData.CityName.Equals(value.CityName))
                    {
                        return gmd.fow.Equals(EnumFogOfWar.Visible);
                    }
                }
            }
        }

        return false;
    }

    internal Vector2Int GetHomeIslandStartingCoordinates()
    {
        return new Vector2Int((StaticTileDataContainer.Instance.mapWidth / 2) - 3, (StaticTileDataContainer.Instance.mapHeight / 2) - 4);
    }
}
