using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public partial class DemoController : MonoBehaviour
{
    public Tilemap tilemapFOW;
    public CinemachineVirtualCamera coastal;
    public CinemachineVirtualCamera openSea;
    public TradeController tradeController;
    public ShipController shipController;

    public ShipStats shipStats;

    [HideInInspector]
    public ShipModelSO currentShipModel;
    public List<ShipModelSO> shipProgressionList;

    private int currentCartographyLevel;
    public List<CartographyLevelSO> cartographyProgressionList;

    public List<RandomMapEventSO> randomMapEventList;

    public List<ArtefactBeacon> artecaftBeaconList;

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

        RandomWorldCreater worldCreator = tilemapFOW.GetComponentInParent<RandomWorldCreater>();

        if (worldCreator != null) 
        {
            worldCreator.GenerateNewworld(gameMapHandler.mapWidth, gameMapHandler.mapHeight);
        }
        // else it shoudl be a static map

        tilemapFOW.gameObject.SetActive(true);

        GameState = EnumGameStates.ShipIdle;
        currentShipModel = shipProgressionList[0];

        currentCartographyLevel = 0;

        PlaceNewShip();
    }

    public void PlaceNewShip()
    {
        // place Ship
        shipStats.Gold = 0;

        SetGameState(EnumGameStates.ShipIdle);
        shipStats.SetShip(currentShipModel);

        shipController.shipCoordinates = gameMapHandler.GetMapStartingCoordinates();
        gameMapHandler.shipCoordinates = shipController.shipCoordinates; // this seems redundant
        shipController.transform.position = tilemapFOW.GetCellCenterWorld((Vector3Int)shipController.shipCoordinates); // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);
        shipController.gameObject.SetActive(true);


        gameMapHandler.NewRun();
    }

    internal ShipModelSO GetCurrentShipModel()
    {
        return currentShipModel;
    }

    internal ShipModelSO GetNextShipModel()
    {    
        int index = shipProgressionList.IndexOf(currentShipModel);

        if (index >= 0 && index < shipProgressionList.Count)
        {
            return shipProgressionList[index + 1];
        }

        return null;
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

    internal CartographyLevelSO GetCurrentCartograhpyLevel()
    {
        if (cartographyProgressionList.Count > currentCartographyLevel)
        {
            return cartographyProgressionList[currentCartographyLevel];
        }

        return null;
    }

    internal CartographyLevelSO GetNextCartograhpyLevel()
    {
        if (cartographyProgressionList.Count > currentCartographyLevel + 1)
        {
            return cartographyProgressionList[currentCartographyLevel + 1];
        }

        return null;
    }

    internal void UpgradeCartographyLevel()
    {
        if (cartographyProgressionList.Count > currentCartographyLevel + 1)
        {
            currentCartographyLevel += 1;
        }
    }
}
