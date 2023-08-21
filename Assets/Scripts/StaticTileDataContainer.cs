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

    public int mapHeight = 63;
    public int mapWidth = 33;

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
        gameMapData = new GameMapData[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                gameMapData[x, y] = new GameMapData(); //.fow = EnumFogOfWar.Undiscovered;
            }
        }
    }
}
