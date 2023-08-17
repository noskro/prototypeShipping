using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class RandomWorldCreater : MonoBehaviour
{
    public List<IslandPrefabController> AvailableIslands;
    public CustomTile tileBackgroundWater;

    public Tilemap tilemapMap;
    public Tilemap tilemapObjects;

    private int mapWidth;
    private int mapHeight;

    public void GenerateNewworld(int width, int height)
    {
        this.mapWidth = width;
        this.mapHeight = height;

        tilemapMap.ClearAllTiles();
        tilemapObjects.ClearAllTiles();

        for (int x = 0; x < width; x++) 
        { 
            for (int y = 0; y < height; y++)
            {
                tilemapMap.SetTile(new Vector3Int(x,y,0), tileBackgroundWater);
            }
        }

        foreach(IslandPrefabController island in AvailableIslands)
        {
            TryPlaceIslandAtRandomPosition(island);
        }
    }

    private void TryPlaceIslandAtRandomPosition(IslandPrefabController island)
    {
        int islandWidth = island.GetTotalWidth();
        int islandHeight = island.GetTotalHeight();

        Tuple<Dictionary<Vector3Int, CustomTile>, Dictionary<Vector3Int, CustomTile>> islandTilesTuple = island.GetAllTiles();

        Dictionary<Vector3Int, CustomTile> islandTilesMap = islandTilesTuple.Item1;
        Dictionary<Vector3Int, CustomTile> islandTilesObjects = islandTilesTuple.Item2;

        int randomX = 0;
        int randomY = 0;
        bool canPlaceIslandHere = true;
        int iAttempts = 0;
        do
        {
            randomX = Mathf.RoundToInt(Random.Range(0, mapWidth - islandWidth)); 
            randomY = (Mathf.RoundToInt(Random.Range(0, mapHeight - islandHeight)) / 2) *2; // only draw to even number columns

            for (int x = randomX; x <= randomX + islandWidth; x++)
            {
                for (int y = randomY; y <= randomY + islandHeight; y++)
                {
                    if (tileBackgroundWater.Equals(tilemapMap.GetTile<CustomTile>(new Vector3Int(x, y, 0))))
                    {
                        // can place
                    }
                    else
                    {
                        canPlaceIslandHere = false;
                        x = int.MaxValue;
                        y = int.MaxValue;
                    }
                }
            }
            iAttempts++;
        } while (iAttempts < 10 && !canPlaceIslandHere);

        if (canPlaceIslandHere)
        {
            foreach(KeyValuePair<Vector3Int, CustomTile> kvp in islandTilesMap)
            {
                tilemapMap.SetTile(kvp.Key + new Vector3Int(randomX, randomY, 0), kvp.Value);
            }
            foreach(KeyValuePair<Vector3Int, CustomTile> kvp in islandTilesObjects)
            {
                tilemapObjects.SetTile(kvp.Key + new Vector3Int(randomX, randomY, 0), kvp.Value);
            }
            //tilemapMap.SetTilesBlock(new BoundsInt(randomX, randomY, 0, islandWidth, islandHeight, 1), islandTilesMap);
            //tilemapObjects.SetTilesBlock(new BoundsInt(randomX, randomY, 0, islandWidth, islandHeight, 1), islandTilesObjects);
        }
    }
}
