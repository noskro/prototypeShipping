using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class IslandPrefab : MonoBehaviour
{
    public EnumIslandUnlockEvent unlockEvent;

    public List<CityDataSO> cityDataList;

    public Tilemap tilemapMap;
    public Tilemap tilemapObjects;

    internal Tuple<Dictionary<Vector3Int, CustomTile>, Dictionary<Vector3Int, CustomTile>> islandTiles;

    private void Awake()
    {
        InitIslandTiles();
    }

    internal void InitIslandTiles()
    { 
        BoundsInt bounds = tilemapMap.cellBounds;

        Dictionary<Vector3Int, CustomTile> customTilesMap = new Dictionary<Vector3Int, CustomTile>();
        Dictionary<Vector3Int, CustomTile> customTilesObjects = new Dictionary<Vector3Int, CustomTile>();

        for (int x = getMinX(); x < bounds.xMin + bounds.size.x; x++)
        {
            for (int y = getMinY(); y < bounds.yMin + bounds.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                CustomTile tile = tilemapMap.GetTile<CustomTile>(position);
                CustomTile tileObjects = tilemapObjects.GetTile<CustomTile>(position);
                if (tile != null)
                {
                    customTilesMap.Add(position - new Vector3Int(getMinX(), getMinY(), 0), tile); 

                    if (tileObjects != null) // write object only if also a map tile is present
                    {
                        customTilesObjects.Add(position - new Vector3Int(getMinX(), getMinY(), 0), tileObjects);
                    }
                }
            }
        }

        islandTiles = new Tuple<Dictionary<Vector3Int, CustomTile>, Dictionary<Vector3Int, CustomTile>>(customTilesMap, customTilesObjects);
    }

    internal int GetTotalWidth()
    {
        if (islandTiles == null || islandTiles.Item1 == null|| islandTiles.Item1.Count == 0) InitIslandTiles();

        int min = int.MaxValue;
        int max = int.MinValue;

        foreach(KeyValuePair<Vector3Int, CustomTile> kvp in islandTiles.Item1)
        {
            min = Mathf.Min(min, kvp.Key.x);
            max = Mathf.Max(max, kvp.Key.x);
        }

        return max - min;
    }

    internal int GetTotalHeight()
    {
        if (islandTiles == null || islandTiles.Item1 == null || islandTiles.Item1.Count == 0) InitIslandTiles();

        int min = int.MaxValue;
        int max = int.MinValue;

        foreach (KeyValuePair<Vector3Int, CustomTile> kvp in islandTiles.Item1)
        {
            min = Mathf.Min(min, kvp.Key.y);
            max = Mathf.Max(max, kvp.Key.y);
        }

        return max - min;
    }

    internal Tuple<Dictionary<Vector3Int, CustomTile>, Dictionary<Vector3Int, CustomTile>> GetAllTiles()
    {
        if (islandTiles == null || islandTiles.Item1 == null || islandTiles.Item1.Count == 0) InitIslandTiles();

        return islandTiles;
    }

    private int getMinX()
    {
        BoundsInt bounds = tilemapMap.cellBounds;
        int MinX = bounds.xMin + bounds.size.x;

        for (int x = bounds.xMin; x < bounds.xMin + bounds.size.x; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMin + bounds.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                CustomTile tile = tilemapMap.GetTile<CustomTile>(position);
                if (tile != null)
                {
                    MinX = Mathf.Min(MinX, x);
                }
            }
        }

        return MinX;
    }

    private int getMinY()
    {
        BoundsInt bounds = tilemapMap.cellBounds;
        int MinY= bounds.yMin + bounds.size.y;

        for (int x = bounds.xMin; x < bounds.xMin + bounds.size.x; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMin + bounds.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                CustomTile tile = tilemapMap.GetTile<CustomTile>(position);
                if (tile != null)
                {
                    MinY = Mathf.Min(MinY, y);
                }
            }
        }

        return MinY;
    }
}
