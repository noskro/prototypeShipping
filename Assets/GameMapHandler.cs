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

    [HideInInspector]
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
                ship.GetComponent<ShipVisual>().ShowShipMoving(false);

                ship.transform.position = shipTargetTransform; // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);

                DiscoverNewAreaByShip(shipCoordinates);
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

    internal CustomTile ClickOnCoords(Vector3Int coordinates)
    {
        coordinates.z = 0;
        if (IsNeighbour(coordinates, shipCoordinates))
        {
            CustomTile objectTile = tilemapObjects.GetTile<CustomTile>(coordinates);
            CustomTile mapTile = tilemapMap.GetTile<CustomTile>(coordinates);

            if (mapTile != null && mapTile.movability.Equals(EnumTileMovability.ShipMoveable))
            {
                shipStartTransform = tilemapFOW.GetCellCenterWorld(shipCoordinates);
                shipTargetTransform = tilemapFOW.GetCellCenterWorld(coordinates);

                ship.GetComponent<ShipVisual>().ShowShipMoving(true);
                IsShipMoving = true;
                shipMovingTimer = 1f;

                shipCoordinates = coordinates;

                return mapTile;
            }
            else if (objectTile != null && objectTile.movability.Equals(EnumTileMovability.TradeVillage) && gameMapData[coordinates.x, coordinates.y].hasVillageTraded == false)
            {
                tradeController.ShowTrade(tilemapObjects.GetCellCenterWorld(coordinates), ref gameMapData[coordinates.x, coordinates.y]);
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
        Vector2Int p0 = HexGridToAxial(center);
        Vector2Int p1 = HexGridToAxial(cell);

        if (p1 == (p0 + new Vector2(1, 0)))
        {
            return true;
        }
        else if (p1 == (p0 + new Vector2(1, -1)))
        {
            return true;
        }
        else if (p1 == (p0 + new Vector2(0, -1)))
        {
            return true;
        }
        else if (p1 == (p0 + new Vector2(-1, 0)))
        {
            return true;
        }
        else if (p1 == (p0 + new Vector2(-1, +1)))
        {
            return true;
        }
        else if (p1 == (p0 + new Vector2(0, 1)))
        {
            return true;
        }

        return false;
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

    internal void DiscoverNewAreaByShip(Vector3Int coords)
    {
        gameMapData[coords.x, coords.y].fow = EnumFogOfWar.Discovered;

        CheckNeighbourForBorderDiscovery(coords.x, coords.y + 1, coords);
        CheckNeighbourForBorderDiscovery(coords.x, coords.y, coords);
        CheckNeighbourForBorderDiscovery(coords.x, coords.y - 1, coords);
        CheckNeighbourForBorderDiscovery(coords.x + 1, coords.y + 1, coords);
        CheckNeighbourForBorderDiscovery(coords.x + 1, coords.y, coords);
        CheckNeighbourForBorderDiscovery(coords.x + 1, coords.y - 1, coords);
        CheckNeighbourForBorderDiscovery(coords.x - 1, coords.y + 1, coords);
        CheckNeighbourForBorderDiscovery(coords.x - 1, coords.y, coords);
        CheckNeighbourForBorderDiscovery(coords.x - 1, coords.y - 1, coords);
    }

    private void CheckNeighbourForBorderDiscovery(int x, int y, Vector3Int center)
    {
        if (x >= 0 && x < gameMapData.GetLength(0) && y >= 0 && y < gameMapData.GetLength(1))
        {
            if (gameMapData[x, y].fow == EnumFogOfWar.Undiscovered)
            {
                if (IsNeighbour(new Vector3Int(x, y, 0), center))
                {
                    gameMapData[x, y].fow = EnumFogOfWar.BorderArea;
                }
            }
        }
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
                    else if (gameMapData[x, y].fow.Equals(EnumFogOfWar.BorderArea))
                    {
                        tilemapFOW.SetTile(new Vector3Int(x, y, 0), tileHalf);
                    }
                }
            }
        }
    }

}
