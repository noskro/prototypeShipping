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
    public CustomTile CustomTileWorldBorder;

    public Transform CityLabelPrefab;

    public int mapRadius;
    //public int mapHeight;
    //public int mapWidth;

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

    public int MapSize
    {
        get
        {
            return mapRadius * 2;
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
        gameMapData = new GameMapData[MapSize, MapSize];
        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                gameMapData[x, y] = new GameMapData(new Vector2Int(x, y)); //.fow = EnumFogOfWar.Undiscovered;
            }
        }
    }

    internal void CreateGameMapDataArrayCircle()
    {
        gameMapData = new GameMapData[MapSize, MapSize];

        Vector2Int center = new Vector2Int(mapRadius, mapRadius);
        List<Vector2Int> circle = GetNeighborsCircle(center, mapRadius, true, false);

        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                if (circle.Contains(new Vector2Int(x, y)))
                {
                    gameMapData[x, y] = new GameMapData(new Vector2Int(x, y)); //.fow = EnumFogOfWar.Undiscovered;
                }
            }
        }
    }

    internal void CreateCircleFromGameMapCenter()
    {
        Vector2Int center = new Vector2Int(mapRadius, mapRadius);
        List<Vector2Int> circle = GetNeighborsCircle(center, mapRadius, false, true);

        foreach (Vector2Int cell in circle)
        {
            StaticTileDataContainer.Instance.TilemapMap.SetTile(new Vector3Int(cell.x, cell.y, 0), StaticTileDataContainer.Instance.CustomTileWorldBorder);
        }
    }

    internal Vector2Int GetMapStartingCoordinates()
    {
        return new Vector2Int((StaticTileDataContainer.Instance.mapRadius), (StaticTileDataContainer.Instance.mapRadius));
    }

    internal void CheckCityDiscovered()
    {
        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                if (StaticTileDataContainer.instance.gameMapData[x, y] != null)
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
    }

    internal bool IsTradedWith(CityDataSO revelantCity)
    {
        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                if (StaticTileDataContainer.instance.gameMapData[x, y] != null)
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
        }

        return false;
    }

    internal void CheckIslandDiscovered()
    {
        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                if (StaticTileDataContainer.instance.gameMapData[x, y] != null)
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
    }

    internal bool IsCityDiscovered(CityDataSO value)
    {
        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                if (StaticTileDataContainer.instance.gameMapData[x, y] != null)
                {
                    GameMapData gmd = gameMapData[x, y];

                    if (gmd == null)
                    {
                        gmd = new GameMapData(new Vector2Int(x, y));
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
        }

        return false;
    }

    internal Vector2Int GetHomeIslandStartingCoordinates()
    {
        return new Vector2Int((StaticTileDataContainer.Instance.mapRadius) - 4, (StaticTileDataContainer.Instance.mapRadius) - 3);
    }

    public List<Vector2Int> GetMoveableNeighbors(Vector2Int unityCell, int range, bool includeSelf = false)
    {
        List<Vector2Int> neighbors = StaticTileDataContainer.GetNeighbors(unityCell, range, includeSelf);
        for (int i = neighbors.Count - 1; i >= 0; i--)
        {
            if (neighbors[i].x < 0 || neighbors[i].y < 0 || neighbors[i].x > StaticTileDataContainer.Instance.MapSize || neighbors[i].y > StaticTileDataContainer.Instance.MapSize ||StaticTileDataContainer.instance.gameMapData[neighbors[i].x, neighbors[i].y] == null)
            {
                neighbors.RemoveAt(i);
            }
            else
            {
                Vector3Int cell = new Vector3Int(neighbors[i].x, neighbors[i].y, 0);
                if (this.TilemapMap.GetTile(cell) == null || !this.TilemapMap.GetTile<CustomTile>(cell).movability.Equals(EnumTileMovability.ShipMoveable))
                {
                    neighbors.RemoveAt(i);
                }
            }
        }

        return neighbors;
    }

    public static List<Vector2Int> GetNeighbors(Vector2Int unityCell, int range, bool includeSelf = false)
    {
        // from https://github.com/Unity-Technologies/2d-extras/issues/69#issuecomment-684190243

        var centerCubePos = UnityCellToCube(unityCell);

        var result = new List<Vector2Int>();

        int min = -range, max = range;

        for (int x = min; x <= max; x++)
        {
            for (int y = min; y <= max; y++)
            {
                var z = -x - y;
                if (z < min || z > max)
                {
                    continue;
                }

                var cubePosOffset = new Vector3Int(x, y, z);
                if (!includeSelf && cubePosOffset == Vector3Int.zero)
                {
                    continue;
                }
                result.Add(CubeToUnityCell(centerCubePos + cubePosOffset));
            }

        }
        return result;
    }

    public static List<Vector2Int> GetNeighborsCircle(Vector2Int unityCell, int range, bool includeSelf = false, bool fixedDistance = true)
    {
        // from https://github.com/Unity-Technologies/2d-extras/issues/69#issuecomment-684190243

        var centerCubePos = UnityCellToCube(unityCell);

        var result = new List<Vector2Int>();

        int min = -range, max = range;

        for (int x = min; x <= max; x++)
        {
            for (int y = min; y <= max; y++)
            {
                var z = -x - y;
                Vector3Int cubePosOffset = new Vector3Int(x, y, z);


                if ((cubePosOffset.magnitude < (range) && fixedDistance) || cubePosOffset.magnitude > range + 1.5f)
                {
                    continue;
                }

                if (!includeSelf && cubePosOffset == Vector3Int.zero)
                {
                    continue;
                }
                result.Add(CubeToUnityCell(centerCubePos + cubePosOffset));
            }

        }
        return result;
    }

    public static Vector3Int UnityCellToCube(Vector2Int cell)
    {
        var yCell = cell.x;
        var xCell = cell.y;
        var x = yCell - (xCell - (xCell & 1)) / 2;
        var z = xCell;
        var y = -x - z;
        return new Vector3Int(x, y, z);
    }

    public static Vector2Int CubeToUnityCell(Vector3Int cube)
    {
        var x = cube.x;
        var z = cube.z;
        var col = x + (z - (z & 1)) / 2;
        var row = z;

        return new Vector2Int(col, row);
    }
}
