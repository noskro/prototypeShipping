using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public partial class DemoController : MonoBehaviour
{
    public Tilemap tilemapFOW;
    public ShipModelSO demoShipModel;
    public ShipStats shipStats;
    public CinemachineVirtualCamera coastal;
    public CinemachineVirtualCamera openSea;
    public TradeController tradeController;
    public ShipController shipController;

    private static DemoController instance;

    public EnumGameStates GameState { get; private set; }

    public delegate void GameStateChanged(EnumGameStates gameStates);
    public static event GameStateChanged OnGameStateChanged;

    public void SetGameState(EnumGameStates newGameState)
    {
        GameState = newGameState;
        OnGameStateChanged?.Invoke(GameState);
    }

    public static DemoController Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
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
        gameMapHandler = GetComponent<GameMapHandler>();
        tilemapFOW.gameObject.SetActive(true);

        GameState = EnumGameStates.ShipIdle;

        PlaceNewShip();
    }

    public void PlaceNewShip()
    {
        // place Ship
        shipStats.Gold = 0;
        shipStats.SetShip(demoShipModel); // these 3 lines need refactoring. The GameState can initially only been set AFTER the ShiStats have been set. But I need to set the ShipStats AFTER setting the GameState, because it uses that value to update the button. Oner call needs to be done manually, but main brain wouldn't do that now
        SetGameState(EnumGameStates.ShipIdle);
        shipStats.SetShip(demoShipModel);

        shipController.shipCoordinates = gameMapHandler.GetMapCenter();
        gameMapHandler.shipCoordinates = shipController.shipCoordinates; // this seems redundant
        shipController.transform.position = tilemapFOW.GetCellCenterWorld((Vector3Int)shipController.shipCoordinates); // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);
        shipController.gameObject.SetActive(true);


        gameMapHandler.NewRun();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void UpdateZoom(ShipStats stats)
    {
        if (gameMapHandler.GetShipTileType() == EnumTileType.CoastalWater)
        {
            Debug.Log("Coastal");
            coastal.gameObject.SetActive(true);
            openSea.gameObject.SetActive(false);
        }
        else if (gameMapHandler.GetShipTileType() == EnumTileType.DeepSea)
        {
            Debug.Log("Open Sea");
            coastal.gameObject.SetActive(false);
            openSea.gameObject.SetActive(true);
        }
    }
}
