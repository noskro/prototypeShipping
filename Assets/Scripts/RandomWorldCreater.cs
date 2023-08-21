using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class RandomWorldCreater : MonoBehaviour
{
    public IslandPrefab HomeIslandPrefab;
    private PersistentIslandData HomeIsland;
    private Vector2Int PositionHomeIsland;

    public List<IslandPrefab> AllExistingIslandPrefabs;

    //private List<IslandPrefabController> AvailableIslands;
    public List<PersistentIslandData> AvailableIslands = new List<PersistentIslandData>();

    private void Start()
    {
        HomeIsland = new PersistentIslandData(HomeIslandPrefab);
        PositionHomeIsland = new Vector2Int(StaticTileDataContainer.Instance.mapWidth / 2 - 3, StaticTileDataContainer.Instance.mapHeight / 2 - 4);
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

        for (int x = 0; x < StaticTileDataContainer.Instance.mapWidth; x++) 
        { 
            for (int y = 0; y < StaticTileDataContainer.Instance.mapHeight; y++)
            {
                StaticTileDataContainer.Instance.TilemapMap.SetTile(new Vector3Int(x,y,0), StaticTileDataContainer.Instance.CustomTileWater);
                StaticTileDataContainer.Instance.gameMapData[x, y].Reset(0);
            }
        }

        TryPlaceIslandAtRandomPosition(HomeIsland, PositionHomeIsland);

        foreach (PersistentIslandData island in AvailableIslands)
        {
            island.Reset(); // reset all run specific temporary data
            TryPlaceIslandAtRandomPosition(island);
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
            if (fixedPosition != null)
            {
                randomX = ((Vector2Int)fixedPosition).x;
                randomY = ((Vector2Int)fixedPosition).y;
            }
            else
            {
                randomX = Mathf.RoundToInt(Random.Range(0, StaticTileDataContainer.Instance.mapWidth - islandWidth));
                randomY = (Mathf.RoundToInt(Random.Range(0, StaticTileDataContainer.Instance.mapHeight - islandHeight)) / 2) * 2; // only draw to even number columns
            }

            for (int x = randomX; x <= randomX + islandWidth; x++)
            {
                for (int y = randomY; y <= randomY + islandHeight; y++)
                {
                    if (StaticTileDataContainer.Instance.CustomTileWater.Equals(StaticTileDataContainer.Instance.TilemapMap.GetTile<CustomTile>(new Vector3Int(x, y, 0))))
                    {
                        // can place
                    }
                    else
                    {
                        canPlaceIslandHere = false;
                        x = randomX + islandWidth + 1;
                        break;
                    }
                }
            }
            iAttempts++;
        } while (iAttempts < 10 && !canPlaceIslandHere);

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

                iCityIndex++;
            }

            StaticTileDataContainer.Instance.UsedIslands.Add(islandData);
        }
    }
}
