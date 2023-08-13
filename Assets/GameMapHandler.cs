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

    public Transform ship;
    public Transform MoveToIcon;
    public Transform CoinIcon;
    public TradeController tradeController;

    public int mapHeight = 63;
    public int mapWidth = 33;
    [HideInInspector]
    public GameMapData[,] gameMapData;

    public Vector3Int shipCoordinates;

    [HideInInspector]
    public bool IsShipMoving = false;
    public float shipMovingTimer = 0f;
    public Vector3 shipStartTransform;
    public Vector3 shipTargetTransform;

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

        IsShipMoving = false;
    }

    public void NewRun()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                gameMapData[x, y].newRun();
            }
        }
    }

    private void Update()
    {
        if (IsShipMoving)
        {
            ship.transform.position = Vector3.Lerp(shipTargetTransform, shipStartTransform, shipMovingTimer);
            shipMovingTimer -= Time.deltaTime * 2;

            if (shipMovingTimer <= 0)
            {
                IsShipMoving = false;
                // ship.GetComponent<ShipVisual>().ShowShipMoving(false);

                ship.transform.position = shipTargetTransform; // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);

                DiscoverNewAreaByShip(shipCoordinates, DemoController.Instance.demoShipModel);
                UpdateFOWMap();
            }
        }
    }

    internal void ShowMouseCursor(Vector3Int cursorCoords)
    {
        cursorCoords.z = 0;
        MoveToIcon.gameObject.SetActive(false);
        CoinIcon.gameObject.SetActive(false);

        if (cursorCoords.x >= 0 && cursorCoords.x < mapWidth && cursorCoords.y >= 0 && cursorCoords.y < mapHeight)
        {
            if (IsNeighbour(cursorCoords, shipCoordinates))
            {
                CustomTile mapTile = tilemapMap.GetTile<CustomTile>(cursorCoords);
                CustomTile objectTile = tilemapObjects.GetTile<CustomTile>(cursorCoords);

                if (mapTile != null && mapTile.movability.Equals(EnumTileMovability.ShipMoveable))
                {
                    MoveToIcon.position = tilemapMap.GetCellCenterWorld(cursorCoords);
                    MoveToIcon.gameObject.SetActive(true);
                }
                else if (objectTile != null && objectTile.movability.Equals(EnumTileMovability.TradeVillage))
                {
                    if (gameMapData[cursorCoords.x, cursorCoords.y].hasVillageTraded == false)
                    {
                        CoinIcon.position = tilemapMap.GetCellCenterWorld(cursorCoords);
                        CoinIcon.gameObject.SetActive(true);
                    }
                }
                else
                {
                    // Debug.Log("Tile movability: " + tilemapMap.GetTile<CustomTile>(cursorCoords).movability);
                }
            }
        }
    }

    internal CustomTile ClickOnCoords(Vector3Int destinationCoords)
    {
        destinationCoords.z = 0;
        if (IsNeighbour(destinationCoords, shipCoordinates))
        {
            CustomTile objectTile = tilemapObjects.GetTile<CustomTile>(destinationCoords);
            CustomTile mapTile = tilemapMap.GetTile<CustomTile>(destinationCoords);

            if (mapTile != null && mapTile.movability.Equals(EnumTileMovability.ShipMoveable))
            {
                shipStartTransform = tilemapFOW.GetCellCenterWorld(shipCoordinates);
                shipTargetTransform = tilemapFOW.GetCellCenterWorld(destinationCoords);

                ship.GetComponent<ShipVisual>().ShowShipMoving(true);
                IsShipMoving = true;
                shipMovingTimer = 1f;

                shipCoordinates = destinationCoords;

                return mapTile;
            }
            else if (objectTile != null && objectTile.movability.Equals(EnumTileMovability.TradeVillage) && gameMapData[destinationCoords.x, destinationCoords.y].hasVillageTraded == false)
            {
                tradeController.ShowTrade(tilemapObjects.GetCellCenterWorld(destinationCoords), ref gameMapData[destinationCoords.x, destinationCoords.y]);
                return null; // no new moved to cell
            }
        }

        return null;
    }


    private Vector2Int HexGridToAxial(Vector3Int pos)
    {
        int c = (int)Mathf.Abs(pos.y) % 2;
        float q = pos.x - (pos.y - c) / 2f;
        float r = pos.y;
        return new Vector2Int((int)Mathf.RoundToInt(q), (int)Mathf.RoundToInt(r));
    }

    private bool IsNeighbour(Vector3Int cell, Vector3Int center)
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

    internal Direction? GetDirection(Vector3Int start, Vector3Int dest)
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

    internal void DiscoverNewAreaByShip(Vector3Int coords, ShipModelSO ship)
    {
        gameMapData[coords.x, coords.y].fow = EnumFogOfWar.Visible;

        var farNeighbors = GetNeighbors(coords, ship.DiscoverRange);
        foreach (var neighbor in farNeighbors)
        {
            if (isWithinMap(neighbor) && gameMapData[neighbor.x, neighbor.y].fow != EnumFogOfWar.Visible) { // don't hide already visible tiles
                gameMapData[neighbor.x, neighbor.y].fow = EnumFogOfWar.Fog;
                tilemapFOW.SetTileFlags(neighbor, TileFlags.None);
                tilemapFOW.SetColor(neighbor, Color.white);
            }
        }
        var nearNeighbors = GetNeighbors(coords, ship.ViewRange);
        foreach (var neighbor in nearNeighbors)
        {
            if (isWithinMap(neighbor))
            {
                gameMapData[neighbor.x, neighbor.y].fow = EnumFogOfWar.Visible;
                tilemapFOW.SetTileFlags(neighbor, TileFlags.None);
                tilemapFOW.SetColor(neighbor, Color.white);
            }
        }
    }

    private bool isWithinMap(Vector3Int coords)
    {
        return coords.x >= 0 && coords.x < mapWidth && coords.y >= 0 && coords.y < mapHeight;
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

    private List<Vector3Int> GetNeighbors(Vector3Int unityCell, int range)
    {
        // from https://github.com/Unity-Technologies/2d-extras/issues/69#issuecomment-684190243

        var centerCubePos = UnityCellToCube(unityCell);

        var result = new List<Vector3Int>();

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
                result.Add(CubeToUnityCell(centerCubePos + cubePosOffset));
            }

        }

        return result;
    }
    private Vector3Int UnityCellToCube(Vector3Int cell)
    {
        var yCell = cell.x;
        var xCell = cell.y;
        var x = yCell - (xCell - (xCell & 1)) / 2;
        var z = xCell;
        var y = -x - z;
        return new Vector3Int(x, y, z);
    }
    private Vector3Int CubeToUnityCell(Vector3Int cube)
    {
        var x = cube.x;
        var z = cube.z;
        var col = x + (z - (z & 1)) / 2;
        var row = z;

        return new Vector3Int(col, row, 0);
    }

}
