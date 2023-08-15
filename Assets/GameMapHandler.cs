using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameMapHandler : MonoBehaviour
{
    public Tilemap tilemapMap;
    public Tilemap tilemapObjects;
    public Tilemap tilemapFOW;

    public RuleTile tileBlack;
    public RuleTile tileHalf;

    //public Tile tileMoveTo;

    public Transform MoveToIcon;
    public Transform CoinIcon;

    public int mapHeight = 63;
    public int mapWidth = 33;
    [HideInInspector]
    public GameMapData[,] gameMapData;

    public Vector2Int shipCoordinates;

    public enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest } // East & West are not used

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

    public void NewRun()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                gameMapData[x, y].Reset(DistanceBetweenCells(new Vector2Int(x,y), GetMapStartingCoordinates()));
            }
        }

        DiscoverNewAreaByShip(shipCoordinates, DemoController.Instance.shipStats.shipModel);
        UpdateFOWMap();
    }

    internal bool CanNavigate(Vector2Int cursorCoords)
    {
        if (cursorCoords.x >= 0 && cursorCoords.x < mapWidth && cursorCoords.y >= 0 && cursorCoords.y < mapHeight)
        {
            if (IsNeighbour(cursorCoords, shipCoordinates))
            {
                CustomTile mapTile = tilemapMap.GetTile<CustomTile>((Vector3Int)cursorCoords);

                if (mapTile != null && mapTile.movability.Equals(EnumTileMovability.ShipMoveable))
                {
                    return true;
                }
            }
        }
        return false;
    }

    internal bool CanTrade(Vector2Int cursorCoords)
    {
        if (cursorCoords.x >= 0 && cursorCoords.x < mapWidth && cursorCoords.y >= 0 && cursorCoords.y < mapHeight)
        {
            if (IsNeighbour(cursorCoords, shipCoordinates))
            {
                CustomTile objectTile = tilemapObjects.GetTile<CustomTile>((Vector3Int)cursorCoords);

                if (objectTile != null && objectTile.movability.Equals(EnumTileMovability.TradeVillage) && gameMapData[cursorCoords.x, cursorCoords.y].hasVillageTraded == false)
                {
                    return true;
                }
            }
        }
        return false;
    }
    internal void ShowMouseCursor(Vector2Int cursorCoords)
    {
        MoveToIcon.gameObject.SetActive(false);
        CoinIcon.gameObject.SetActive(false);

        if (CanNavigate(cursorCoords))
        {
            MoveToIcon.position = tilemapMap.GetCellCenterWorld((Vector3Int)cursorCoords);
            MoveToIcon.gameObject.SetActive(true);
        }
        else if (CanTrade(cursorCoords))
        {
            CoinIcon.position = tilemapMap.GetCellCenterWorld((Vector3Int)cursorCoords);
            CoinIcon.gameObject.SetActive(true);
        }
    }


    internal void SetDestination(Vector2Int destinationCoords)
    {
        if (IsNeighbour(destinationCoords, shipCoordinates))
        {
            if (CanNavigate(destinationCoords))
            {
                shipCoordinates = destinationCoords;
            }
        }
    }


    private Vector2Int HexGridToAxial(Vector2Int pos)
    {
        int c = (int)Mathf.Abs(pos.y) % 2;
        float q = pos.x - (pos.y - c) / 2f;
        float r = pos.y;
        return new Vector2Int((int)Mathf.RoundToInt(q), (int)Mathf.RoundToInt(r));
    }

    private bool IsNeighbour(Vector2Int cell, Vector2Int center)
    {
        if (GetNeighbors(center, 1).Contains(cell))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    internal Direction? GetDirection(Vector2Int start, Vector2Int dest)
    {
        Vector2Int p0 = HexGridToAxial(start);
        Vector2Int p1 = HexGridToAxial(dest);

        if (p1 == (p0 + new Vector2(1, 0)))
        {
            return Direction.North;
        }
        else if (p1 == (p0 + new Vector2(1, -1)))
        {
            return Direction.NorthWest;
        }
        else if (p1 == (p0 + new Vector2(0, -1)))
        {
            return Direction.SouthWest;
        }
        else if (p1 == (p0 + new Vector2(-1, 0)))
        {
            return Direction.South;
        }
        else if (p1 == (p0 + new Vector2(-1, +1)))
        {
            return Direction.SouthEast;
        }
        else if (p1 == (p0 + new Vector2(0, 1)))
        {
            return Direction.NorthEast;
        }

        return null;
    }

    internal void DiscoverNewAreaByShip(Vector2Int coords, ShipModelSO ship)
    {
        gameMapData[coords.x, coords.y].fow = EnumFogOfWar.Visible;

        var farNeighbors = GetNeighbors(coords, ship.DiscoverRange);
        foreach (var neighbor in farNeighbors)
        {
            if (IsWithinMap(neighbor) && gameMapData[neighbor.x, neighbor.y].fow != EnumFogOfWar.Visible)
            { // don't hide already visible tiles
                gameMapData[neighbor.x, neighbor.y].fow = EnumFogOfWar.Fog;
                tilemapFOW.SetTileFlags((Vector3Int)neighbor, TileFlags.None);
                tilemapFOW.SetColor((Vector3Int)neighbor, Color.white);
            }
        }
        var nearNeighbors = GetNeighbors(coords, ship.ViewRange);
        foreach (Vector2Int neighbor in nearNeighbors)
        {
            if (IsWithinMap(neighbor))
            {
                gameMapData[neighbor.x, neighbor.y].fow = EnumFogOfWar.Visible;
                tilemapFOW.SetTileFlags((Vector3Int)neighbor, TileFlags.None);
                tilemapFOW.SetColor((Vector3Int)neighbor, Color.white);
            }
        }

        UpdateFOWMap();
    }

    internal bool IsWithinMap(Vector2Int coords)
    {
        return coords != null && coords.x >= 0 && coords.x < mapWidth && coords.y >= 0 && coords.y < mapHeight;
    }

    public EnumTileType GetShipTileType()
    {
        return tilemapMap.GetTile<CustomTile>((Vector3Int)shipCoordinates).type;
    }


    internal void UpdateFOWMap()
    {
        if (tilemapFOW != null)
        {
            tilemapFOW.ClearAllTiles();

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (gameMapData[x, y].fow.Equals(EnumFogOfWar.Undiscovered))
                    {
                        tilemapFOW.SetTile(new Vector3Int(x, y, 0), tileBlack);
                    }
                    else if (gameMapData[x, y].fow.Equals(EnumFogOfWar.Fog))
                    {
                        tilemapFOW.SetTile(new Vector3Int(x, y, 0), tileHalf);
                    }
                }
            }
        }
    }

    private List<Vector2Int> GetNeighbors(Vector2Int unityCell, int range, bool includeSelf = false)
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
    private Vector3Int UnityCellToCube(Vector2Int cell)
    {
        var yCell = cell.x;
        var xCell = cell.y;
        var x = yCell - (xCell - (xCell & 1)) / 2;
        var z = xCell;
        var y = -x - z;
        return new Vector3Int(x, y, z);
    }
    private Vector2Int CubeToUnityCell(Vector3Int cube)
    {
        var x = cube.x;
        var z = cube.z;
        var col = x + (z - (z & 1)) / 2;
        var row = z;

        return new Vector2Int(col, row);
    }

    private float DistanceBetweenCells(Vector2Int cellA, Vector2Int cellB)
    {
        int xd = cellA.x - cellB.x;
        int yd = cellA.y - cellB.y;
        float dist = MathF.Sqrt((xd*xd) + (yd*yd)); 



        //int xd = cellA.x - cellB.x;
        //int yd = cellA.y - ((cellA.x + (cellA.x & 1)) / 2) - (cellB.y - (cellB.x + (cellB.x & 1)) / 2);
        //int dist = (Mathf.Abs(xd) + Mathf.Abs(xd + yd) + Mathf.Abs(yd)) / 2;
        return Mathf.RoundToInt(dist);
    }

    internal Vector2Int GetMapStartingCoordinates()
    {
        return new Vector2Int(mapWidth / 2, mapHeight / 2);
    }

    internal Vector2Int GetCellCoords(Vector3 mouseWorldPosition)
    {
        return (Vector2Int)tilemapMap.WorldToCell(mouseWorldPosition);
    }

    internal Vector3 GetCellPosition(Vector2Int mouseCellCoordinates)
    {
        return tilemapMap.GetCellCenterWorld((Vector3Int)mouseCellCoordinates);
    }

    internal CustomTile GetMapTile(Vector2Int mouseCellCoordinates)
    {
        return tilemapMap.GetTile<CustomTile>((Vector3Int)mouseCellCoordinates);
    }
}
