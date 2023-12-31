using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class RandomWorldCreater : MonoBehaviour
{
    public int baseWorldRadius;
    public int maxAttemptsToPlaceIsland;
    
    public IslandPrefab HomeIslandPrefab;
    public PersistentIslandData HomeIsland;
    private Vector2Int PositionHomeIsland;

    public List<IslandPrefab> AllExistingIslandPrefabs;

    //private List<IslandPrefabController> AvailableIslands;
    public List<PersistentIslandData> AvailableIslands = new List<PersistentIslandData>();

    private void Start()
    {
        // create home island data
        HomeIsland = new PersistentIslandData(HomeIslandPrefab);
        HomeIsland.IslandDiscovered = true;
        HomeIsland.PersistentCityDataList[0].DocksBuild = true;
        HomeIsland.PersistentCityDataList[0].BeaconBuild = true;

        PositionHomeIsland = StaticTileDataContainer.Instance.GetHomeIslandStartingCoordinates();
    }

    public void AddNewIslandPrefabsToAvailable(EnumIslandUnlockEvent unlockEvent)
    {
        // get the new IslandPrefabs, create a PersistentIslandData for each and fill it city CityDataSO for each city. Then add it to availableIslands
        //foreach(IslandPrefab islandPrefab in AllExistingIslandPrefabs)
        for(int i = AllExistingIslandPrefabs.Count - 1; i >= 0; i--)
        {
            IslandPrefab islandPrefab = AllExistingIslandPrefabs[i];
            if (unlockEvent.Equals(islandPrefab.unlockEvent))
            {
                PersistentIslandData islandData = new PersistentIslandData(islandPrefab);
                AvailableIslands.Add(islandData);
                AllExistingIslandPrefabs.RemoveAt(i);
            }
        }
    }

    public void GenerateNewWorld()
    {
        StaticTileDataContainer.Instance.TilemapMap.ClearAllTiles();
        StaticTileDataContainer.Instance.TilemapObjects.ClearAllTiles();

        foreach (Transform child in StaticTileDataContainer.Instance.TilemapObjects.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int x = 0; x < StaticTileDataContainer.Instance.MapSize; x++)
        {
            for (int y = 0; y < StaticTileDataContainer.Instance.MapSize; y++)
            {
                if (StaticTileDataContainer.Instance.gameMapData[x, y] != null)
                {
                    StaticTileDataContainer.Instance.gameMapData[x, y].Reset(0);
                }
            }
        }

        StaticTileDataContainer.Instance.mapRadius = baseWorldRadius + AvailableIslands.Count * 5; // for reasons of rendering the height must be the double of odd number (e.g 30 = 15*2) / start value is 4 islands * 4 = 16 + 14 = 30

        //StaticTileDataContainer.Instance.CreateGameMapDataArray();
        StaticTileDataContainer.Instance.CreateGameMapDataArrayCircle();

        for (int x = 0; x < StaticTileDataContainer.Instance.MapSize; x++)
        {
            for (int y = 0; y < StaticTileDataContainer.Instance.MapSize; y++)
            {
                if (StaticTileDataContainer.Instance.gameMapData[x, y] != null)
                {
                    StaticTileDataContainer.Instance.TilemapMap.SetTile(new Vector3Int(x, y, 0), StaticTileDataContainer.Instance.CustomTileWater);
                }
                else
                {
                    StaticTileDataContainer.Instance.TilemapMap.SetTile(new Vector3Int(x, y, 0), null);
                }
                //if (x >= 0 && y >= 0 && x < StaticTileDataContainer.Instance.mapWidth && y < StaticTileDataContainer.Instance.mapHeight)
                //{
                //    if (StaticTileDataContainer.Instance.gameMapData[x, y] == null)
                //    {
                //        StaticTileDataContainer.Instance.gameMapData[x, y] = new GameMapData(new Vector2Int(x, y)); // on map extension, create new array entry
                //                                                                                                    // set pathfindingNode
                //    }
                //}
            }
        }

        // calculate distance for circle  from center
        StaticTileDataContainer.Instance.CreateCircleFromGameMapCenter();


        PositionHomeIsland = StaticTileDataContainer.Instance.GetHomeIslandStartingCoordinates();
        TryPlaceIslandAtRandomPosition(HomeIsland, PositionHomeIsland);

        //foreach (PersistentIslandData island in AvailableIslands.Reverse())
        for (int i = AvailableIslands.Count - 1; i >= 0; i--) // better sort by size later. lartges islands should be placed first... or random? I don't know
        {
            PersistentIslandData island = AvailableIslands[i];
            island.Reset(); // reset all run specific temporary data
            TryPlaceIslandAtRandomPosition(island);
        }

        // set pathfinding tile notes
        for (int x = 0; x < StaticTileDataContainer.Instance.MapSize; x++)
        {
            for (int y = 0; y < StaticTileDataContainer.Instance.MapSize; y++)
            {
                if (StaticTileDataContainer.Instance.gameMapData[x, y] != null)
                {
                    CustomTile tile = StaticTileDataContainer.Instance.TilemapMap.GetTile<CustomTile>(new Vector3Int(x, y, 0));
                    StaticTileDataContainer.Instance.gameMapData[x, y].SetPathfindingNodeMoveable(tile.movability.Equals(EnumTileMovability.ShipMoveable));

                    List<Vector2Int> neighbors = StaticTileDataContainer.Instance.GetMoveableNeighbors(new Vector2Int(x, y), 1);
                    List<PathfindingTileNode> neighborTileNodes = new List<PathfindingTileNode>();
                    foreach(Vector2Int n in neighbors)
                    {
                        try
                        {
                            neighborTileNodes.Add(StaticTileDataContainer.Instance.gameMapData[n.x, n.y].pathfingingTileNode);
                        }
                        catch (Exception e)
                        {
                            int a = 0;
                        }
                    }
                    StaticTileDataContainer.Instance.gameMapData[x, y].SetPathfindingNodeNeighbors(neighborTileNodes);
                }
            }
        }

    }

    private void TryPlaceIslandAtRandomPosition(PersistentIslandData islandData, Vector2Int? fixedPosition = null)
    {
        StaticTileDataContainer.Instance.UsedIslands.Clear();

        IslandPrefab islandPrefab = islandData.prefab;
        int islandWidth = islandPrefab.GetTotalWidth();
        int islandHeight = islandPrefab.GetTotalHeight();

        Tuple<Dictionary<Vector3Int, CustomTile>, Dictionary<Vector3Int, CustomTile>> islandTilesTuple = islandPrefab.GetAllTiles();

        Dictionary<Vector3Int, CustomTile> islandTilesMap = islandTilesTuple.Item1;
        Dictionary<Vector3Int, CustomTile> islandTilesObjects = islandTilesTuple.Item2;

        int randomX = 0;
        int randomY = 0;
        bool canPlaceIslandHere = true;
        int iAttempts = 0;
        do
        {
            canPlaceIslandHere = true;
            if (fixedPosition != null)
            {
                randomX = ((Vector2Int)fixedPosition).x;
                randomY = ((Vector2Int)fixedPosition).y;
            }
            else
            {
                randomX = Mathf.RoundToInt(Random.Range(0, StaticTileDataContainer.Instance.MapSize - islandWidth));
                randomY = (Mathf.RoundToInt(Random.Range(0, StaticTileDataContainer.Instance.MapSize - islandHeight)) / 2) * 2; // only draw to even number columns
            }

            foreach(KeyValuePair<Vector3Int, CustomTile> kbpTile in islandTilesTuple.Item1)
            {
                if (StaticTileDataContainer.Instance.CustomTileWater.Equals(StaticTileDataContainer.Instance.TilemapMap.GetTile<CustomTile>(kbpTile.Key + new Vector3Int(randomX, randomY, 0))))
                {
                    // can place on deep water
                }
                // Active to place island is coastal water of other islands
                //else if (StaticTileDataContainer.Instance.CustomTileCoastalWater.Equals(StaticTileDataContainer.Instance.TilemapMap.GetTile<CustomTile>(kbpTile.Key + new Vector3Int(randomX, randomY, 0))) &&
                //        StaticTileDataContainer.Instance.CustomTileCoastalWater.Equals(kbpTile.Value))
                //{
                //    // can place coastal water on existing coastal water
                //}
                else
                {
                    canPlaceIslandHere = false;
                    break;
                }
            }

            //for (int x = randomX; x <= randomX + islandWidth; x++)
            //{
            //    for (int y = randomY; y <= randomY + islandHeight; y++)
            //    {
            //        if ()
            //        if (StaticTileDataContainer.Instance.CustomTileWater.Equals(StaticTileDataContainer.Instance.TilemapMap.GetTile<CustomTile>(new Vector3Int(x, y, 0))))
            //        {
            //            // can place
            //        }
            //        else
            //        {
            //            canPlaceIslandHere = false;
            //            x = randomX + islandWidth + 1;
            //            break;
            //        }
            //    }
            //}
            iAttempts++;
        } while (iAttempts < maxAttemptsToPlaceIsland && !canPlaceIslandHere);

        if (canPlaceIslandHere)
        {
            foreach(KeyValuePair<Vector3Int, CustomTile> kvp in islandTilesMap)
            {
                StaticTileDataContainer.Instance.TilemapMap.SetTile(kvp.Key + new Vector3Int(randomX, randomY, 0), kvp.Value);
            }

            int iCityIndex = 0;
            foreach(KeyValuePair<Vector3Int, CustomTile> kvp in islandTilesObjects)
            {
                StaticTileDataContainer.Instance.TilemapObjects.SetTile(kvp.Key + new Vector3Int(randomX, randomY, 0), kvp.Value);
                StaticTileDataContainer.Instance.gameMapData[kvp.Key.x + randomX, kvp.Key.y + randomY].setCityData(islandData, iCityIndex); // .SetTile(kvp.Key + new Vector3Int(randomX, randomY, 0), kvp.Value);

                // show city labels
                Transform newCityLabel = Instantiate(StaticTileDataContainer.Instance.CityLabelPrefab, StaticTileDataContainer.Instance.TilemapObjects.transform);
                newCityLabel.position = StaticTileDataContainer.Instance.TilemapObjects.CellToWorld(new Vector3Int(kvp.Key.x + randomX, kvp.Key.y + randomY, 0));
                newCityLabel.GetComponent<CityLabelView>().SetCityData(islandData.PersistentCityDataList[iCityIndex]);
                StaticTileDataContainer.Instance.gameMapData[kvp.Key.x + randomX, kvp.Key.y + randomY].setCityNameOverlay(newCityLabel.GetComponent<CityLabelView>()); // .SetTile(kvp.Key + new Vector3Int(randomX, randomY, 0), kvp.Value);

                iCityIndex++;
            }

            StaticTileDataContainer.Instance.UsedIslands.Add(islandData);
        }
        else
        {
            Debug.Log("Could not place island: " + islandData.prefab.name);
        }
    }
}
