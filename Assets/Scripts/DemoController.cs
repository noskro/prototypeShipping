using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public partial class DemoController : MonoBehaviour
{
    public CinemachineVirtualCamera coastal;
    public CinemachineVirtualCamera openSea;

    private static DemoController instance;

    public bool IsStoryTextShown;

    public EnumGameStates GameState;

    public delegate void GameStateChanged(EnumGameStates gameStates);
    public static event GameStateChanged OnGameStateChanged;

    public TradeController tradeController;
    public ShipController shipController;

    public GameMapHandler gameMapHandler;
    public ShipBattleManager shipBattleManager;
    //public ShipStats shipStats;

    //public ShipModelSO currentShipModel;
    public List<ShipModelSO> shipProgressionList;

    //private int currentCartographyLevel;
    //public List<CartographyLevelSO> cartographyProgressionList;

    public List<RandomMapEventSO> randomMapEventList;
    public List<RandomMapEventSO> pirateDefeatEventList;

    //public List<ArtefactBeacon> artecaftBeaconList;
    public RandomWorldCreater worldCreator;
    public MetaUpgradeUI metaUpgrade;
    public StoryTextManager storyTextManager;

    public List<EnumStoryProgressionTags> StoryProgressionTags;

    public int DamagePerCanon;

    public int Run = 0;
    public int FieldsTravelled = 0;

    public void SetGameState(EnumGameStates newGameState)
    {
        GameState = newGameState;

        if (GameState.Equals(EnumGameStates.ShipLost))
        {
            //metaUpgrade.Show();
        }

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
        if (StaticTileDataContainer.Instance != null && StaticTileDataContainer.Instance.TilemapFOW != null)
        {
            worldCreator = StaticTileDataContainer.Instance.TilemapFOW.GetComponentInParent<RandomWorldCreater>();
        }
    }

    private void OnEnable()
    {
        ShipController.OnShipUpdated += UpdateZoom;
    }

    private void OnDisable()
    {
        ShipController.OnShipUpdated -= UpdateZoom;
    }

    // Start is called before the first frame update
    void Start()
    {
        Run = 0;
        gameMapHandler = GetComponent<GameMapHandler>();

        if (worldCreator != null)
        {
            worldCreator.AddNewIslandPrefabsToAvailable(EnumIslandUnlockEvent.StarterIsland);
        }
        // else it shoudl be a static map

        //StaticTileDataContainer.Instance.TilemapFOW.gameObject.SetActive(true);
        shipController.shipStats.SetShipModel(shipProgressionList[0]);
        //currentCartographyLevel = 0;
        shipController.shipStats.Gold = 0;

        GenerateNewRun();
    }

    public void GenerateNewRun()
    {
        Run ++;
        FieldsTravelled = 0;

        StaticTileDataContainer.Instance.TilemapFOW.gameObject.SetActive(true);
        RandomWorldCreater worldCreator = StaticTileDataContainer.Instance.TilemapFOW.GetComponentInParent<RandomWorldCreater>();

        if (worldCreator != null)
        {
            worldCreator.GenerateNewWorld();
        }

        PlaceNewShip();
    }

    private void PlaceNewShip()
    {
        gameMapHandler.NewRun();

        // place Ship
        SetGameState(EnumGameStates.Start);
        shipController.shipStats.SetShipModel(shipController.shipStats.shipModel);

        gameMapHandler.shipCoordinates = StaticTileDataContainer.Instance.GetMapStartingCoordinates(); 
        shipController.transform.position = StaticTileDataContainer.Instance.TilemapFOW.GetCellCenterWorld((Vector3Int)gameMapHandler.shipCoordinates); // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);
        shipController.gameObject.SetActive(true);

        shipController.TriggerShipUpdated(); 
        gameMapHandler.StartRun();
    }

    internal ShipModelSO GetCurrentShipModel()
    {
        return shipController.shipStats.shipModel;
    }

    internal ShipModelSO GetNextShipModel()
    {    
        int index = shipProgressionList.IndexOf(shipController.shipStats.shipModel);

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
            coastal.gameObject.SetActive(true);
            openSea.gameObject.SetActive(false);
        }
        else if (gameMapHandler.GetShipTileType() == EnumTileType.DeepSea)
        {
            coastal.gameObject.SetActive(false);
            openSea.gameObject.SetActive(true);
        }
    }

    internal CartographyLevelSO GetCurrentCartograhpyLevel()
    {
        //if (cartographyProgressionList.Count > currentCartographyLevel)
        //{
        //    return cartographyProgressionList[currentCartographyLevel];
        //}

        return null;
    }

    internal CartographyLevelSO GetNextCartograhpyLevel()
    {
        //if (cartographyProgressionList.Count > currentCartographyLevel + 1)
        //{
        //    return cartographyProgressionList[currentCartographyLevel + 1];
        //}

        return null;
    }

    internal void UpgradeCartographyLevel()
    {
        //if (cartographyProgressionList.Count > currentCartographyLevel + 1)
        //{
        //    currentCartographyLevel += 1;
        //}
    }

    internal void ShowGameScene()
    {
        
    }
}
