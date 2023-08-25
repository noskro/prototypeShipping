using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public partial class GameMapHandler : MonoBehaviour
{
    public Transform AttackIcon;
    public Transform MoveToIcon;
    public Transform CoinIcon;

    public Vector2Int shipCoordinates;

    public List<PirateShipController> pirateShips;

    public void NewRun()
    {
        //for (int x = 0; x < StaticTileDataContainer.Instance.mapWidth; x++)
        //{
        //    for (int y = 0; y < StaticTileDataContainer.Instance.mapHeight; y++)
        //    {
        //        StaticTileDataContainer.Instance.gameMapData[x, y].Reset(DistanceBetweenCells(new Vector2Int(x,y), GetMapStartingCoordinates()));
        //    }
        //}

        // place random events
        foreach (RandomMapEventSO e in DemoController.Instance.randomMapEventList)
        {
            int randomOccurence = Random.Range(e.EventMinOccurence, e.EventMaxOccurence);

            if (randomOccurence > 0)
            {
                for (int i = 0; i < randomOccurence; i++)
                {
                    bool placed = false;
                    int iAttempts = 0;
                    do
                    {
                        int x = Random.Range(0, StaticTileDataContainer.Instance.mapWidth);
                        int y = Random.Range(0, StaticTileDataContainer.Instance.mapHeight);

                        if (e.placedOnAnyTile.Contains(GetMapTile(x, y)) && GetObjectTile(x, y) == null)
                        {
                            StaticTileDataContainer.Instance.TilemapObjects.SetTile(new Vector3Int(x, y, 0), e.eventTile);
                            placed = true;
                        }
                        iAttempts++;
                    } while (iAttempts <= 10 && placed == false);
                }
            }
        }

        // place random beacon
        //foreach (ArtefactBeacon beacon in DemoController.Instance.artecaftBeaconList)
        //{
        //    int randomX = Random.Range(0, StaticTileDataContainer.Instance.mapWidth);
        //    int randomY = Random.Range(0, StaticTileDataContainer.Instance.mapHeight);
        //    beacon.PlaceBeacon(new Vector2Int(randomX, randomY), StaticTileDataContainer.Instance.TilemapMap.CellToWorld(new Vector3Int(randomX, randomY, 0)));

        //    beacon.SetShipCoordinates(StaticTileDataContainer.Instance.TilemapMap, shipCoordinates);
        //}

        // reset pirates
        foreach (PirateShipController pirate in pirateShips)
        {
            pirate.ResetForNewRun();
        }


        DiscoverNewAreaByShip(shipCoordinates, DemoController.Instance.shipController.shipStats);
        UpdateFOWMap();
    }

    internal bool IsValidTarget(Vector2Int targetCell)
    {
        CustomTile mapTile = StaticTileDataContainer.Instance.TilemapMap.GetTile<CustomTile>((Vector3Int)targetCell);

        if (mapTile != null && mapTile.movability.Equals(EnumTileMovability.ShipMoveable))
        {
            return true;
        }

        return false;
    }

    internal void UpdateBeacons(Vector2Int shipCoordinates)
    {
        //foreach (ArtefactBeacon beacon in DemoController.Instance.artecaftBeaconList)
        //{
        //    beacon.SetShipCoordinates(StaticTileDataContainer.Instance.TilemapMap, shipCoordinates);
        //}
    }

    internal void HandleRandomEvents(Vector2Int shipCoordinates)
    {
        CustomTile tile = GetObjectTile(shipCoordinates.x, shipCoordinates.y); 

        if (tile != null)
        {
            foreach(RandomMapEventSO e in DemoController.Instance.randomMapEventList)
            { 
                if (e.eventTile.Equals(tile))
                {
                    // event triggered;
                    int reResultIndex = Random.Range(0, e.RandomEventResultList.Count);
                    RandomEventResult reResult = e.RandomEventResultList[reResultIndex];

                    DemoController.Instance.shipController.shipStats.AddStatsModifier(reResult);
                    
                    if (!e.EventRepeatable)
                    {
                        StaticTileDataContainer.Instance.TilemapObjects.SetTile(new Vector3Int(shipCoordinates.x, shipCoordinates.y, 0), null);
                    }
                }
            }

            foreach(RandomMapEventSO e in DemoController.Instance.pirateDefeatEventList)
            { 
                if (e.eventTile.Equals(tile))
                {
                    // event triggered;
                    int reResultIndex = Random.Range(0, e.RandomEventResultList.Count);
                    RandomEventResult reResult = e.RandomEventResultList[reResultIndex];

                    DemoController.Instance.shipController.shipStats.AddStatsModifier(reResult);
                    
                    StaticTileDataContainer.Instance.TilemapObjects.SetTile(new Vector3Int(shipCoordinates.x, shipCoordinates.y, 0), null);
                }
            }
        }

    }

    internal bool CanNavigate(Vector2Int targetCoord)
    {
        return CanNavigate(targetCoord, shipCoordinates);
    }

    internal bool CanNavigate(Vector2Int targetCoord, Vector2Int sourceCoord)
    {
        if (targetCoord.x >= 0 && targetCoord.x <StaticTileDataContainer.Instance.mapWidth && targetCoord.y >= 0 && targetCoord.y < StaticTileDataContainer.Instance.mapHeight)
        {
            if (IsNeighbour(sourceCoord, targetCoord))
            {
                CustomTile mapTile = StaticTileDataContainer.Instance.TilemapMap.GetTile<CustomTile>((Vector3Int)targetCoord);

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
        if (cursorCoords.x >= 0 && cursorCoords.x <StaticTileDataContainer.Instance.mapWidth && cursorCoords.y >= 0 && cursorCoords.y < StaticTileDataContainer.Instance.mapHeight)
        {
            if (IsNeighbour(cursorCoords, shipCoordinates))
            {
                CustomTile objectTile = StaticTileDataContainer.Instance.TilemapObjects.GetTile<CustomTile>((Vector3Int)cursorCoords);

                if (objectTile != null && objectTile.movability.Equals(EnumTileMovability.TradeVillage) && StaticTileDataContainer.Instance.gameMapData[cursorCoords.x, cursorCoords.y].hasVillageTraded() == false)
                {
                    return true;
                }
            }
        }
        return false;
    }

    internal void HideAllMouseCursor()
    {
        AttackIcon.gameObject.SetActive(false);
        MoveToIcon.gameObject.SetActive(false);
        CoinIcon.gameObject.SetActive(false);
    }

    internal void ShowMouseCursor(Vector2Int cursorCoords)
    {
        HideAllMouseCursor();

        if (CanNavigate(cursorCoords))
        {
            // check for pirates
            if (PiratesPresent(cursorCoords) != null)
            {
                AttackIcon.position = StaticTileDataContainer.Instance.TilemapMap.GetCellCenterWorld((Vector3Int)cursorCoords);
                AttackIcon.gameObject.SetActive(true);
            }
            else
            {
                Direction? direction = this.GetDirection(shipCoordinates, cursorCoords);

                int rotation = 0;
                if (direction.Equals(Direction.South))
                {
                    rotation = 180;
                }
                else if (direction.Equals(Direction.NorthEast))
                {
                    rotation = 300;
                }
                else if (direction.Equals(Direction.SouthEast))
                {
                    rotation = 240;
                }
                else if (direction.Equals(Direction.NorthWest))
                {
                    rotation = 60;
                }
                else if (direction.Equals(Direction.SouthWest))
                {
                    rotation = 120;
                }

                MoveToIcon.transform.localRotation = Quaternion.Euler(0, 0, rotation);

                MoveToIcon.position = StaticTileDataContainer.Instance.TilemapMap.GetCellCenterWorld((Vector3Int)cursorCoords);
                MoveToIcon.gameObject.SetActive(true);
            }
        }
        else if (CanTrade(cursorCoords))
        {
            CoinIcon.position = StaticTileDataContainer.Instance.TilemapMap.GetCellCenterWorld((Vector3Int)cursorCoords);
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

    internal void DiscoverNewAreaByShip(Vector2Int coords, ShipStats ship)
    {
        DiscoverNewAreaByShip(coords, ship.GetCurrentDiscoverRange(), ship.GetCurrentViewRange());
    }

    internal void DiscoverNewAreaByShip(Vector2Int coords, int discoverRange, int viewRange)
    {
        StaticTileDataContainer.Instance.gameMapData[coords.x, coords.y].fow = EnumFogOfWar.Visible;

        var farNeighbors = GetNeighbors(coords, discoverRange);
        foreach (var neighbor in farNeighbors)
        {
            if (IsWithinMap(neighbor) && StaticTileDataContainer.Instance.gameMapData[neighbor.x, neighbor.y].fow != EnumFogOfWar.Visible)
            { // don't hide already visible tiles
                StaticTileDataContainer.Instance.gameMapData[neighbor.x, neighbor.y].fow = EnumFogOfWar.Fog;
                StaticTileDataContainer.Instance.TilemapFOW.SetTileFlags((Vector3Int)neighbor, TileFlags.None);
                StaticTileDataContainer.Instance.TilemapFOW.SetColor((Vector3Int)neighbor, Color.white);
            }
        }
        var nearNeighbors = GetNeighbors(coords, viewRange);
        foreach (Vector2Int neighbor in nearNeighbors)
        {
            if (IsWithinMap(neighbor))
            {
                StaticTileDataContainer.Instance.gameMapData[neighbor.x, neighbor.y].fow = EnumFogOfWar.Visible;
                StaticTileDataContainer.Instance.TilemapFOW.SetTileFlags((Vector3Int)neighbor, TileFlags.None);
                StaticTileDataContainer.Instance.TilemapFOW.SetColor((Vector3Int)neighbor, Color.white);
            }
        }

        UpdateFOWMap();
        foreach (TilemapMask mask in StaticTileDataContainer.Instance.TilemapFOW.GetComponents<TilemapMask>())
        {
            mask.DoIt();
        }

        StaticTileDataContainer.Instance.CheckCityDiscovered();
        StaticTileDataContainer.Instance.CheckIslandDiscovered();
    }

    internal bool IsWithinMap(Vector2Int coords)
    {
        return coords != null && coords.x >= 0 && coords.x < StaticTileDataContainer.Instance.mapWidth && coords.y >= 0 && coords.y < StaticTileDataContainer.Instance.mapHeight;
    }

    public EnumTileType GetShipTileType()
    {
        return StaticTileDataContainer.Instance.TilemapMap.GetTile<CustomTile>((Vector3Int)shipCoordinates).type;
    }


    internal void UpdateFOWMap()
    {
        if (StaticTileDataContainer.Instance.TilemapFOW != null)
        {
            StaticTileDataContainer.Instance.TilemapFOW.ClearAllTiles();

            for (int x = 0; x <StaticTileDataContainer.Instance.mapWidth; x++)
            {
                for (int y = 0; y <StaticTileDataContainer.Instance.mapHeight; y++)
                {
                    if (StaticTileDataContainer.Instance.gameMapData[x, y].CityData != null && StaticTileDataContainer.Instance.gameMapData[x, y].CityData.BeaconBuild)
                    {
                        // discover cities with beacon and fog the surroundings if they are still undiscovered
                        StaticTileDataContainer.Instance.gameMapData[x, y].fow = EnumFogOfWar.Visible;

                        var neighbors = GetNeighbors(new Vector2Int(x, y), 1);
                        foreach (var neighbor in neighbors)
                        {
                            if (StaticTileDataContainer.Instance.gameMapData[neighbor.x, neighbor.y].fow.Equals(EnumFogOfWar.Undiscovered))
                            {
                                StaticTileDataContainer.Instance.gameMapData[neighbor.x, neighbor.y].fow = EnumFogOfWar.Fog;
                            }
                        }
                    }

                    if (StaticTileDataContainer.Instance.gameMapData[x, y].fow.Equals(EnumFogOfWar.Undiscovered))
                    {
                        StaticTileDataContainer.Instance.TilemapFOW.SetTile(new Vector3Int(x, y, 0), StaticTileDataContainer.Instance.CustomTileBlack);
                    }
                    else if (StaticTileDataContainer.Instance.gameMapData[x, y].fow.Equals(EnumFogOfWar.Fog))
                    {
                        StaticTileDataContainer.Instance.TilemapFOW.SetTile(new Vector3Int(x, y, 0), StaticTileDataContainer.Instance.CustomTileBlackFog);
                    }
                }
            }

            // create FoW for the outside border
            for (int x = -1; x < StaticTileDataContainer.Instance.mapWidth+1; x++)
            {
                int y = -1;
                StaticTileDataContainer.Instance.TilemapFOW.SetTile(new Vector3Int(x, y, 0), StaticTileDataContainer.Instance.CustomTileBlack);

                y = StaticTileDataContainer.Instance.mapHeight;
                StaticTileDataContainer.Instance.TilemapFOW.SetTile(new Vector3Int(x, y, 0), StaticTileDataContainer.Instance.CustomTileBlack);
            }
            for (int y = 0; y < StaticTileDataContainer.Instance.mapHeight; y++)
            {
                int x = -1;
                StaticTileDataContainer.Instance.TilemapFOW.SetTile(new Vector3Int(x, y, 0), StaticTileDataContainer.Instance.CustomTileBlack);

                x = StaticTileDataContainer.Instance.mapWidth;
                StaticTileDataContainer.Instance.TilemapFOW.SetTile(new Vector3Int(x, y, 0), StaticTileDataContainer.Instance.CustomTileBlack);
            }

                    //// copy for 8 other sides
                    //StaticTileDataContainer.Instance.TilemapFOW.CompressBounds();
                    //BoundsInt bound = StaticTileDataContainer.Instance.TilemapFOW.cellBounds;
                    //TileBase[] originalTiles = StaticTileDataContainer.Instance.TilemapFOW.GetTilesBlock(bound);

                    //BoundsInt newBound = new BoundsInt(bound.position - new Vector3Int(StaticTileDataContainer.Instance.mapHeight, 0, 0), bound.size);
                    //StaticTileDataContainer.Instance.TilemapFOW.SetTilesBlock(newBound, originalTiles);

                    //newBound = new BoundsInt(bound.position + new Vector3Int(StaticTileDataContainer.Instance.mapHeight, 0, 0), bound.size);
                    //StaticTileDataContainer.Instance.TilemapFOW.SetTilesBlock(newBound, originalTiles);

                    //newBound = new BoundsInt(bound.position - new Vector3Int(0, StaticTileDataContainer.Instance.mapWidth, 0), bound.size);
                    //StaticTileDataContainer.Instance.TilemapFOW.SetTilesBlock(newBound, originalTiles);

                    //newBound = new BoundsInt(bound.position + new Vector3Int(0, StaticTileDataContainer.Instance.mapWidth, 0), bound.size);
                    //StaticTileDataContainer.Instance.TilemapFOW.SetTilesBlock(newBound, originalTiles);

                    //newBound = new BoundsInt(bound.position - new Vector3Int(StaticTileDataContainer.Instance.mapHeight, StaticTileDataContainer.Instance.mapWidth, 0), bound.size);
                    //StaticTileDataContainer.Instance.TilemapFOW.SetTilesBlock(newBound, originalTiles);

                    //newBound = new BoundsInt(bound.position - new Vector3Int(StaticTileDataContainer.Instance.mapHeight, -1 * StaticTileDataContainer.Instance.mapWidth, 0), bound.size);
                    //StaticTileDataContainer.Instance.TilemapFOW.SetTilesBlock(newBound, originalTiles);

                    //newBound = new BoundsInt(bound.position + new Vector3Int(StaticTileDataContainer.Instance.mapHeight, StaticTileDataContainer.Instance.mapWidth, 0), bound.size);
                    //StaticTileDataContainer.Instance.TilemapFOW.SetTilesBlock(newBound, originalTiles);

                    //newBound = new BoundsInt(bound.position + new Vector3Int(StaticTileDataContainer.Instance.mapHeight, -1 * StaticTileDataContainer.Instance.mapWidth, 0), bound.size);
                    //StaticTileDataContainer.Instance.TilemapFOW.SetTilesBlock(newBound, originalTiles);

        }
    }

    public List<Vector2Int> GetNeighbors(Vector2Int unityCell, int range, bool includeSelf = false)
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

    internal Vector2Int GetCellCoords(Vector3 mouseWorldPosition)
    {
        return (Vector2Int)StaticTileDataContainer.Instance.TilemapMap.WorldToCell(mouseWorldPosition);
    }

    internal Vector3 GetCellPosition(Vector2Int mouseCellCoordinates)
    {
        return StaticTileDataContainer.Instance.TilemapMap.GetCellCenterWorld((Vector3Int)mouseCellCoordinates);
    }

    internal CustomTile GetObjectTile(int x, int y)
    {
        return StaticTileDataContainer.Instance.TilemapObjects.GetTile<CustomTile>(new Vector3Int(x, y, 0));
    }

    internal CustomTile GetMapTile(int x, int y)
    {
        return GetMapTile(new Vector2Int(x, y));
    }

    internal CustomTile GetMapTile(Vector2Int mouseCellCoordinates)
    {
        return StaticTileDataContainer.Instance.TilemapMap.GetTile<CustomTile>((Vector3Int)mouseCellCoordinates);
    }

    internal void HandleOtherShips()
    {
        foreach(PirateShipController pirateShip in pirateShips)
        {
            pirateShip.PerformAction();
        }
    }

    public void SetPirateShipLost(ShipStats defendingShip)
    {
        foreach (PirateShipController pirate in pirateShips)
        {
            if (pirate.shipStats.Equals(defendingShip))
            {
                pirate.SetPirateShipLost();
            }
        }
    }

    internal ShipStats PiratesPresent(Vector2Int mouseCellCoordinates)
    {
        foreach(PirateShipController pirate in pirateShips)
        {
            if (!pirate.pirateShipState.Equals(EnumGameStates.ShipLost) && pirate.pirateShipCoordinates.Equals(mouseCellCoordinates))
            {
                return pirate.shipStats;
            }
        }

        return null;
    }
}
