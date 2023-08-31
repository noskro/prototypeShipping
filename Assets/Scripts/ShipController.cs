using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipController : MonoBehaviour
{
    public float ShipMovementAnimationSpeed;

    public GameMapHandler gameMapHandler;
    public ShipStats shipStats;
    public ShipStatsUI shipStatusUI;
    private SpriteRenderer shipSpriteRenderer;
    private TradeController tradeController;
    private DemoController demoController;
    //public Vector2Int shipCoordinates;

    public Vector3 shipStartPosition;
    public Vector3 shipTargetPosition;
    public List<Vector2Int> nextScheduledTargets;
    public bool doShipRotate;
    public float shipMovingTimer = 0f;

    public delegate void ShipUpdated(ShipStats stats);
    public static event ShipUpdated OnShipUpdated;

    // Start is called before the first frame update
    void Start()
    {
        shipStats = GetComponent<ShipStats>();
        shipSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        tradeController = DemoController.Instance.tradeController;
        demoController = DemoController.Instance;
    }

    //private void OnEnable()
    //{
    //    ShipStats.OnShipUpdated += UpdateShip;
    //}

    //private void OnDisable()
    //{
    //    ShipStats.OnShipUpdated -= UpdateShip;
    //}

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        if (demoController.GameState == EnumGameStates.ShipMoving)
        {
            transform.position = Vector3.Lerp(shipTargetPosition, shipStartPosition, shipMovingTimer);
            shipMovingTimer -= Time.deltaTime * ShipMovementAnimationSpeed;

            if (shipMovingTimer <= 0)
            {
                EndShipMovement();
            }
        }
        else if (nextScheduledTargets.Count != 0 && (demoController.GameState == EnumGameStates.ShipIdle || demoController.GameState == EnumGameStates.Start))
        {
            Vector2Int nexttarget = nextScheduledTargets[0];
            Vector2Int currentShipCoords = gameMapHandler.shipCoordinates;
            gameMapHandler.SetDestination(nexttarget);
            shipStartPosition = transform.position;
            shipTargetPosition = gameMapHandler.GetCellPosition(nexttarget);

            demoController.SetGameState(EnumGameStates.ShipMoving);
            shipMovingTimer = 1f;

            CustomTile targetTile = gameMapHandler.GetMapTile(nexttarget);
            shipStats.direction = gameMapHandler.GetDirection(currentShipCoords, nexttarget);
            shipStats.NextTurn(targetTile);
            TriggerShipUpdated();
            demoController.FieldsTravelled++;


            Debug.Log("... next Target selected: " + nexttarget.x + ", " + nexttarget.y);
            nextScheduledTargets.RemoveAt(0);
        }
    }


    void HandleInput()
    {
        gameMapHandler.HideAllMouseCursor();

        if (demoController.shipBattleManager.battleInProgress)
        {
            // no game input while in battle
        }
        else if (demoController.GameState == EnumGameStates.ShipLost)
        {
            // no game input while ship is lost
        }
        else if (tradeController.IsTrading) // maybe handle this in DemoController instead?
        {
            // handled by tradeController
        }
        else if (DemoController.Instance.IsStoryTextShown) // maybe handle this in DemoController instead?
        {
            // nio input while story text is shown
        }
        else
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int mouseCellCoordinates = gameMapHandler.GetCellCoords(mouseWorldPosition);

            if (gameMapHandler.IsWithinMap(mouseCellCoordinates))
            {
                gameMapHandler.ShowMouseCursor(mouseCellCoordinates);

                if (Input.GetMouseButton(0))
                {
                    DemoController.Instance.shipController.shipStatusUI.ShowPossibleStatChangeString(null,null,null, null, null, null);
                    Vector2Int currentShipCoords = gameMapHandler.shipCoordinates;

                    if (gameMapHandler.CanNavigate(mouseCellCoordinates))
                    {
                        // check for pirates
                        if (gameMapHandler.PiratesPresent(mouseCellCoordinates) != null)
                        {
                            if (demoController.GameState != EnumGameStates.ShipMoving)
                            {
                                if (shipStats.GetCurrentCanons() > 0)
                                {
                                    DemoController.Instance.shipBattleManager.StartShipBattle(shipStats, gameMapHandler.PiratesPresent(mouseCellCoordinates));
                                }
                            }
                        }
                        else
                        {
                            if (demoController.GameState == EnumGameStates.ShipMoving)
                            {
                                if (nextScheduledTargets.Count == 0 && mouseCellCoordinates != gameMapHandler.shipCoordinates)
                                {
                                    nextScheduledTargets.Add(mouseCellCoordinates);
                                    Debug.Log("One new target scheudled.");
                                }
                                //else
                                //{
                                //    // if ship moves and there are more targets, the next click will cancel those targets
                                //    nextScheduledTargets.Clear();
                                //    Debug.Log("Next target scheudle cleared.");
                                //}
                            }
                            else
                            {
                                gameMapHandler.SetDestination(mouseCellCoordinates);
                                shipStartPosition = transform.position;
                                shipTargetPosition = gameMapHandler.GetCellPosition(mouseCellCoordinates);

                                demoController.SetGameState(EnumGameStates.ShipMoving);
                                shipMovingTimer = 1f;

                                CustomTile targetTile = gameMapHandler.GetMapTile(mouseCellCoordinates);
                                shipStats.direction = gameMapHandler.GetDirection(currentShipCoords, mouseCellCoordinates);
                                shipStats.NextTurn(targetTile);
                                demoController.FieldsTravelled++;
                            }
                        }
                    }
                    else if (gameMapHandler.CanTrade(mouseCellCoordinates))
                    {
                        if (demoController.GameState != EnumGameStates.ShipMoving)
                        {
                            gameMapHandler.CoinIcon.gameObject.SetActive(false);
                            transform.position = gameMapHandler.GetCellPosition(mouseCellCoordinates);
                            tradeController.ShowTrade(gameMapHandler.GetCellPosition(mouseCellCoordinates), StaticTileDataContainer.Instance.gameMapData[mouseCellCoordinates.x, mouseCellCoordinates.y].CityData);
                        }
                    }
                    else if (currentShipCoords != mouseCellCoordinates) // check pathfinding route
                    {
                        if (nextScheduledTargets.Count == 0 && demoController.GameState != EnumGameStates.ShipMoving)
                        {
                            PathfindingTileNode start = StaticTileDataContainer.Instance.gameMapData[currentShipCoords.x, currentShipCoords.y].pathfingingTileNode;
                            PathfindingTileNode end = StaticTileDataContainer.Instance.gameMapData[mouseCellCoordinates.x, mouseCellCoordinates.y].pathfingingTileNode;
                            List<PathfindingTileNode> path = Pathfinding.FindPath(start, end);

                            //foreach(PathfindingTileNode tile in path)
                            for (int i = 1; i < path.Count; i++)
                            {
                                nextScheduledTargets.Add(StaticTileDataContainer.CubeToUnityCell(path[i].cubePosition));
                                //Debug.Log(StaticTileDataContainer.CubeToUnityCell(tile.cubePosition));
                                Debug.DrawLine(gameMapHandler.GetCellPosition(StaticTileDataContainer.CubeToUnityCell(path[i - 1].cubePosition)), gameMapHandler.GetCellPosition(StaticTileDataContainer.CubeToUnityCell(path[i].cubePosition)), Color.red, 1f);
                            }

                            Debug.Log("New targets scheduled: " + nextScheduledTargets.Count);
                        }
                    }
                }
            }
        }

        TriggerShipUpdated();
    }

    internal void ResetForNewRun()
    {
        nextScheduledTargets.Clear();
        shipStats.SetShipModel(this.shipStats.shipModel);
        transform.position = StaticTileDataContainer.Instance.TilemapFOW.GetCellCenterWorld((Vector3Int)gameMapHandler.shipCoordinates); // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);
        gameObject.SetActive(true);
        TriggerShipUpdated();
    }

    private void EndShipMovement()
    {
        Debug.Log("Target reached: " + gameMapHandler.shipCoordinates.x + ", " + gameMapHandler.shipCoordinates.y + ". Targets Scheuded: " + nextScheduledTargets.Count);

        shipMovingTimer = 0;

        transform.position = shipTargetPosition; // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);

        gameMapHandler.DiscoverNewAreaByShip(gameMapHandler.shipCoordinates, this.shipStats);

        gameMapHandler.HandleRandomEvents(gameMapHandler.shipCoordinates);

        EndTurn();
    }

    public void EndTurn()
    {
        gameMapHandler.HandleOtherShips();

        demoController.SetGameState(EnumGameStates.ShipIdle);

        DemoController.Instance.storyTextManager.CheckForNewEvents();
    }

    internal void ResetShipPosition()
    {
        transform.position = shipTargetPosition;
        TriggerShipUpdated();
    }


    public void TriggerShipUpdated()
    {
        OnShipUpdated?.Invoke(shipStats);
    }
}

