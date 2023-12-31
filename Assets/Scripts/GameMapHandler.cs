using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public partial class GameMapHandler : MonoBehaviour
{
    public Transform AttackIcon;
    public Transform MoveToIcon;
    public List<Transform> pathfindingDots;
    public Transform CoinIcon;

    public Vector2Int shipCoordinates;

    public List<PirateShipController> pirateShips;

    public void NewRun()
    {
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
                        int x = Random.Range(0, StaticTileDataContainer.Instance.MapSize);
                        int y = Random.Range(0, StaticTileDataContainer.Instance.MapSize);

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
    }

    public void StartRun()
    { 
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

        DemoController.Instance.shipController.TriggerShipUpdated();
    }

    internal bool CanNavigate(Vector2Int targetCoord)
    {
        return CanNavigate(targetCoord, shipCoordinates);
    }

    internal bool CanNavigate(Vector2Int targetCoord, Vector2Int sourceCoord)
    {
        if (IsWithinMap(targetCoord)) // StaticTileDataContainer.Instance.gameMapData[targetCoord.x, targetCoord.y] != null) // targetCoord.x >= 0 && targetCoord.x < StaticTileDataContainer.Instance.MapSize && targetCoord.y >= 0 && targetCoord.y < StaticTileDataContainer.Instance.MapSize)
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
        if (IsWithinMap(cursorCoords)) // StaticTileDataContainer.Instance.gameMapData[cursorCoords.x, cursorCoords.y] != null) //cursorCoords.x >= 0 && cursorCoords.x <StaticTileDataContainer.Instance.mapWidth && cursorCoords.y >= 0 && cursorCoords.y < StaticTileDataContainer.Instance.mapHeight)
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

        for (int i = 0; i < pathfindingDots.Count; i++)
        {
            pathfindingDots[i].gameObject.SetActive(false);
        }
    }

    internal void ShowMouseCursor(Vector2Int cursorCoords)
    {
        HideAllMouseCursor();
        DemoController.Instance.shipController.shipStatusUI.ShowPossibleStatChange(null, null, null, null, null, null);

        if (CanNavigate(cursorCoords))
        {
            // check for pirates
            ShipStats possiblePirate = PiratesPresent(cursorCoords);
            if (possiblePirate != null)
            {
                if (DemoController.Instance.shipController.shipStats.GetCurrentCanons() <= 0)
                {
                    return;
                }

                AttackIcon.position = StaticTileDataContainer.Instance.TilemapMap.GetCellCenterWorld((Vector3Int)cursorCoords);
                AttackIcon.gameObject.SetActive(true);

                int attackDamage = possiblePirate.getMaxAttackDamage();
                int attackDamageHalf = Mathf.FloorToInt(attackDamage / 2);
                DemoController.Instance.shipController.shipStatusUI.ShowPossibleStatChangeString(
                    "-(" + attackDamageHalf +"-"+ attackDamage + ")", 
                    "-(" + Mathf.CeilToInt(attackDamageHalf/10) + "-" + Mathf.CeilToInt(attackDamage / 10) + ")", 
                    null, null, null, null);
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

                CustomTile targetTile = StaticTileDataContainer.Instance.TilemapMap.GetTile<CustomTile>((Vector3Int)cursorCoords);

                showShipStatsChange(targetTile, cursorCoords);
            }
        }
        else if (CanTrade(cursorCoords))
        {
            CoinIcon.position = StaticTileDataContainer.Instance.TilemapMap.GetCellCenterWorld((Vector3Int)cursorCoords);
            CoinIcon.gameObject.SetActive(true);
        }
        else // show pathfinding
        {
            PathfindingTileNode start = StaticTileDataContainer.Instance.gameMapData[shipCoordinates.x, shipCoordinates.y].pathfingingTileNode;
            PathfindingTileNode end = StaticTileDataContainer.Instance.gameMapData[cursorCoords.x, cursorCoords.y].pathfingingTileNode;
            List<PathfindingTileNode> path = Pathfinding.FindPath(start, end);

            if (path.Count > 1)
            {
                for (int i = 0; i < pathfindingDots.Count; i++)
                {
                    if (i <= path.Count - 2)
                    {
                        Direction? direction = this.GetDirection(path[i].cellPosition, path[i + 1].cellPosition);

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

                        if (i < path.Count - 2)
                        {
                            pathfindingDots[i].transform.localRotation = Quaternion.Euler(0, 0, rotation);

                            pathfindingDots[i].position = StaticTileDataContainer.Instance.TilemapMap.GetCellCenterWorld((Vector3Int)path[i+1].cellPosition);
                            pathfindingDots[i].gameObject.SetActive(true);
                        }
                        else if (i == path.Count - 2 || pathfindingDots.Count < path.Count)
                        {
                            i = path.Count - 1; // always draw the last path element direction on the real compass
                            MoveToIcon.transform.localRotation = Quaternion.Euler(0, 0, rotation);

                            MoveToIcon.position = StaticTileDataContainer.Instance.TilemapMap.GetCellCenterWorld((Vector3Int)cursorCoords);
                            MoveToIcon.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        break;
                    }
                }


                //CustomTile targetTile = StaticTileDataContainer.Instance.TilemapMap.GetTile<CustomTile>((Vector3Int)cursorCoords);
                //showShipStatsChange(targetTile, cursorCoords);
            }
        }
    }

    private void showShipStatsChange(CustomTile targetTile, Vector2Int cursorCoords)
    {
        int shipDamageMin = 0;
        int shipDamageMax = 0;

        int shipCanonsMax = 0;
        int shipCrewMax = 0;

        int goldMax = 0;

        if (StaticTileDataContainer.Instance.CustomTileWater.Equals(targetTile))
        {
            shipDamageMin = -1 * DemoController.Instance.shipController.shipStats.shipDamageAtSea;
        }
        else if (StaticTileDataContainer.Instance.CustomTileCoastalWater.Equals(targetTile))
        {
            shipDamageMin = -1 * DemoController.Instance.shipController.shipStats.shipDamageAtCoast;
        }

        shipDamageMax = shipDamageMin;

        CustomTile tile = GetObjectTile(cursorCoords.x, cursorCoords.y);
        // check for random events
        if (tile != null)
        {
            shipDamageMax = 0;
            shipDamageMin = 0;
            foreach (RandomMapEventSO e in DemoController.Instance.randomMapEventList)
            {
                if (e.eventTile.Equals(tile))
                {
                    foreach (RandomEventResult reResult in e.RandomEventResultList)
                    {
                        if (reResult.type.Equals(EnumEventModifierRewardType.Durability))
                        {
                            shipDamageMax += Mathf.CeilToInt(reResult.valueMax);
                        }
                        if (reResult.type.Equals(EnumEventModifierRewardType.Crew))
                        {
                            shipCrewMax += Mathf.CeilToInt(reResult.valueMax);
                        }
                        if (reResult.type.Equals(EnumEventModifierRewardType.Gold))
                        {
                            goldMax += Mathf.CeilToInt(reResult.valueMax);
                        }
                    }
                }
            }

            foreach (RandomMapEventSO e in DemoController.Instance.pirateDefeatEventList)
            {
                if (e.eventTile.Equals(tile))
                {
                    foreach (RandomEventResult reResult in e.RandomEventResultList)
                    {
                        if (reResult.type.Equals(EnumEventModifierRewardType.Durability))
                        {
                            shipDamageMax += Mathf.CeilToInt(reResult.valueMax);
                        }
                        if (reResult.type.Equals(EnumEventModifierRewardType.Canons))
                        {
                            shipCanonsMax += Mathf.CeilToInt(reResult.valueMax);
                        }
                    }
                }
            }
        }

        string sDamage = (shipDamageMax == shipDamageMin) ? 
                            "" + shipDamageMax :
                            (shipDamageMax > 0) ? 
                                "+(" + shipDamageMin + "-" + shipDamageMax + ")" : 
                                (shipDamageMax < 0) ? 
                                    "-(" + Mathf.Abs(shipDamageMin) + "-" + Mathf.Abs(shipDamageMax) + ")": 
                                    null;
        string sCrew = (shipCrewMax > 0) ? "+(0-" + shipCrewMax + ")" : (shipCrewMax < 0) ? "-(0-" + Mathf.Abs(shipCrewMax) + ")" : null;
        string sCanon = (shipCanonsMax > 0) ? "+(0-" + shipCanonsMax + ")" : (shipCanonsMax < 0) ? "-(0-" + Mathf.Abs(shipCanonsMax) + ")" : null;
        string sGold = (goldMax > 0) ? "+(0-" + goldMax + ")" : (goldMax < 0) ? "-(0-" + Mathf.Abs(goldMax) + ")" : null;

        DemoController.Instance.shipController.shipStatusUI.ShowPossibleStatChangeString(
            sDamage,
            sCrew, null, null, 
            sCanon,
            sGold);
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
        if (StaticTileDataContainer.GetNeighbors(center, 1).Contains(cell))
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

        var farNeighbors = StaticTileDataContainer.GetNeighbors(coords, discoverRange);
        foreach (var neighbor in farNeighbors)
        {
            if (IsWithinMap(neighbor) && StaticTileDataContainer.Instance.gameMapData[neighbor.x, neighbor.y].fow != EnumFogOfWar.Visible)
            { // don't hide already visible tiles
                StaticTileDataContainer.Instance.gameMapData[neighbor.x, neighbor.y].fow = EnumFogOfWar.Fog;
                StaticTileDataContainer.Instance.TilemapFOW.SetTileFlags((Vector3Int)neighbor, TileFlags.None);
                StaticTileDataContainer.Instance.TilemapFOW.SetColor((Vector3Int)neighbor, Color.white);
            }
        }
        var nearNeighbors = StaticTileDataContainer.GetNeighbors(coords, viewRange);
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
        return coords != null && StaticTileDataContainer.Instance.gameMapData[coords.x, coords.y] != null; //coords.x >= 0 && coords.x < StaticTileDataContainer.Instance.mapWidth && coords.y >= 0 && coords.y < StaticTileDataContainer.Instance.mapHeight;
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

            for (int x = 0; x <StaticTileDataContainer.Instance.MapSize; x++)
            {
                for (int y = 0; y <StaticTileDataContainer.Instance.MapSize; y++)
                {
                    if (StaticTileDataContainer.Instance.gameMapData[x, y] == null)
                    {
                        continue;
                        //StaticTileDataContainer.Instance.gameMapData[x, y] = new GameMapData(new Vector2Int(x,y));
                    }

                    StaticTileDataContainer.Instance.gameMapData[x, y].pathfingingTileNode.isDiscovered = true;

                    if (StaticTileDataContainer.Instance.gameMapData[x, y].CityData != null && StaticTileDataContainer.Instance.gameMapData[x, y].CityData.BeaconBuild)
                    {
                        // discover cities with beacon and fog the surroundings if they are still undiscovered
                        StaticTileDataContainer.Instance.gameMapData[x, y].fow = EnumFogOfWar.Visible;

                        var neighbors = StaticTileDataContainer.GetNeighbors(new Vector2Int(x, y), 1);
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
                        StaticTileDataContainer.Instance.gameMapData[x, y].pathfingingTileNode.isDiscovered = false;
                    }
                    else if (StaticTileDataContainer.Instance.gameMapData[x, y].fow.Equals(EnumFogOfWar.Fog))
                    {
                        StaticTileDataContainer.Instance.TilemapFOW.SetTile(new Vector3Int(x, y, 0), StaticTileDataContainer.Instance.CustomTileBlackFog);
                    }
                }
            }

            // create FoW for the outside border
            //for (int x = -1; x < StaticTileDataContainer.Instance.mapWidth+1; x++)
            //{
            //    int y = -1;
            //    StaticTileDataContainer.Instance.TilemapFOW.SetTile(new Vector3Int(x, y, 0), StaticTileDataContainer.Instance.CustomTileBlack);

            //    y = StaticTileDataContainer.Instance.mapHeight;
            //    StaticTileDataContainer.Instance.TilemapFOW.SetTile(new Vector3Int(x, y, 0), StaticTileDataContainer.Instance.CustomTileBlack);
            //}
            //for (int y = 0; y < StaticTileDataContainer.Instance.mapHeight; y++)
            //{
            //    int x = -1;
            //    StaticTileDataContainer.Instance.TilemapFOW.SetTile(new Vector3Int(x, y, 0), StaticTileDataContainer.Instance.CustomTileBlack);

            //    x = StaticTileDataContainer.Instance.mapWidth;
            //    StaticTileDataContainer.Instance.TilemapFOW.SetTile(new Vector3Int(x, y, 0), StaticTileDataContainer.Instance.CustomTileBlack);
            //}
        }
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
