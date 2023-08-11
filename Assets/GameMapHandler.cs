using System;
using System.Collections;
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
    EnumFogOfWar[,] fowMap;

    [HideInInspector]
    public Vector3Int shipCoordinates;

    [HideInInspector]
    public bool IsShipMooving = false;
    public float shipMovingTimer = 0f;
    public Vector3 shipStartTransform;
    public Vector3 shipTargetTransform;

    // Start is called before the first frame update
    void Start()
    {
        fowMap = new EnumFogOfWar[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                fowMap[x,y] = EnumFogOfWar.Undiscovered;
            }
        }

        IsShipMooving = false;
    }

    private void Update()
    {
        if (IsShipMooving)
        {
            ship.transform.position = Vector3.Lerp(shipTargetTransform, shipStartTransform, shipMovingTimer);
            shipMovingTimer -= Time.deltaTime * 2;

            if (shipMovingTimer <= 0)
            {
                IsShipMooving = false;
                ship.GetComponent<ShipVisual>().ShowShipMoving(false);

                ship.transform.position = shipTargetTransform; // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);

                DiscoverNewAreaByShip(shipCoordinates);
                UpdateFOWMap();
            }
        }
    }

    internal CustomTile ClickOnCoords(Vector3Int coordinates)
    {
        coordinates.z = 0;
        if (IsNeighbour(coordinates, shipCoordinates))
        {
            if (((CustomTile)tilemapMap.GetTile(coordinates)).movability.Equals(EnumTileMovability.ShipMoveable))
            {
                shipStartTransform = tilemapFOW.GetCellCenterWorld(shipCoordinates);
                shipTargetTransform = tilemapFOW.GetCellCenterWorld(coordinates);

                ship.GetComponent<ShipVisual>().ShowShipMoving(true);
                IsShipMooving = true;
                shipMovingTimer = 1f;

                shipCoordinates = coordinates;

                return (CustomTile)tilemapMap.GetTile(coordinates);
            }
            else if (((CustomTile)tilemapObjects.GetTile(coordinates)).movability.Equals(EnumTileMovability.TradeVillage))
            {
                tradeController.ShowTrade(tilemapObjects.GetCellCenterWorld(coordinates));
                return null; // no new moved to cell
            }
        }

        return null;
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
                Vector3 worldPosition = tilemapMap.GetCellCenterWorld(cursorCoords);

                if (tilemapMap.GetTile<CustomTile>(cursorCoords) != null && tilemapMap.GetTile<CustomTile>(cursorCoords).movability.Equals(EnumTileMovability.ShipMoveable))
                {
                    MoveToIcon.position = worldPosition;
                    MoveToIcon.gameObject.SetActive(true);
                }
                else if (tilemapObjects.GetTile<CustomTile>(cursorCoords) != null && tilemapObjects.GetTile<CustomTile>(cursorCoords).movability.Equals(EnumTileMovability.TradeVillage))
                {
                    Debug.Log("Village");
                    CoinIcon.position = worldPosition;
                    CoinIcon.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("Tile movability: " + tilemapMap.GetTile<CustomTile>(cursorCoords).movability);
                }
            }
        }
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

    internal void DiscoverNewAreaByShip(Vector3Int coords)
    {
        fowMap[coords.x, coords.y] = EnumFogOfWar.Discovered;

        CheckNeighbourForBorderDiscovery(coords.x, coords.y+1, coords);
        CheckNeighbourForBorderDiscovery(coords.x, coords.y, coords);
        CheckNeighbourForBorderDiscovery(coords.x, coords.y-1, coords);
        CheckNeighbourForBorderDiscovery(coords.x+1, coords.y+1, coords);
        CheckNeighbourForBorderDiscovery(coords.x+1, coords.y, coords);
        CheckNeighbourForBorderDiscovery(coords.x+1, coords.y-1, coords);
        CheckNeighbourForBorderDiscovery(coords.x-1, coords.y+1, coords);
        CheckNeighbourForBorderDiscovery(coords.x-1, coords.y, coords);
        CheckNeighbourForBorderDiscovery(coords.x-1, coords.y-1, coords);
    }

    private void CheckNeighbourForBorderDiscovery(int x, int y, Vector3Int center)
    {
        if (x >= 0 && x < fowMap.GetLength(0) && y >= 0 && y < fowMap.GetLength(1))
        {
            if (fowMap[x, y] == EnumFogOfWar.Undiscovered)
            {
                if (IsNeighbour(new Vector3Int(x, y, 0), center))
                {
                    fowMap[x, y] = EnumFogOfWar.BorderArea;
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
                    if (fowMap[x, y].Equals(EnumFogOfWar.Undiscovered))
                    {
                        tilemapFOW.SetTile(new Vector3Int(x, y, 0), tileBlack);
                    }
                    else if (fowMap[x, y].Equals(EnumFogOfWar.BorderArea))
                    {
                        tilemapFOW.SetTile(new Vector3Int(x, y, 0), tileHalf);
                    }
                }
            }
        }
    }

}
