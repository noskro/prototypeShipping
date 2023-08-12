using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DemoController : MonoBehaviour
{
    public Tilemap tilemapFOW;
    public ShipModelSO demoShipModel;
    public ShipStats shipStats;
    public CinemachineVirtualCamera coastal;
    public CinemachineVirtualCamera openSea;

    private static DemoController instance;

    public static DemoController Instance
    {
        get
        {
            return instance;
        }
    }

    private void OnEnable()
    {
        ShipStats.OnShipUpdated += UpdateZoom;
    }

    private void OnDisable()
    {
        ShipStats.OnShipUpdated -= UpdateZoom;
    }

    private GameMapHandler gameMapHandler;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        gameMapHandler = GetComponent<GameMapHandler>();
        tilemapFOW.gameObject.SetActive(true);

        PlaceNewShip();
    }

    public void PlaceNewShip()
    {
        gameMapHandler.NewRun();

        gameMapHandler.shipCoordinates = new Vector3Int((int)Mathf.Round(gameMapHandler.mapWidth / 2), (int)Mathf.Round(gameMapHandler.mapHeight / 2), 0);
        gameMapHandler.ship.transform.position = tilemapFOW.GetCellCenterWorld(gameMapHandler.shipCoordinates); // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);
        gameMapHandler.ship.gameObject.SetActive(true);
        gameMapHandler.DiscoverNewAreaByShip(gameMapHandler.shipCoordinates);
        gameMapHandler.UpdateFOWMap();

        // place Ship
        shipStats.Gold = 0;
        shipStats.SetShip(demoShipModel);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void UpdateZoom()
    {
        if (gameMapHandler.tilemapMap.GetTile<CustomTile>(gameMapHandler.shipCoordinates).type == EnumTileType.CoastalWater)
        {
            Debug.Log("Coastal");
            coastal.gameObject.SetActive(true);
            openSea.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Open Sea");
            coastal.gameObject.SetActive(false);
            openSea.gameObject.SetActive(true);
        }
    }
}
