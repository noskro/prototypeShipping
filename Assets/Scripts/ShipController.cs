using System;
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
    public Vector2Int? nextScheduledTarget = null;
    public bool doShipRotate;
    public float shipMovingTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        shipStats = GetComponent<ShipStats>();
        shipSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        tradeController = DemoController.Instance.tradeController;
        demoController = DemoController.Instance;
    }

    private void OnEnable()
    {
        ShipStats.OnShipUpdated += UpdateShip;
    }

    private void OnDisable()
    {
        ShipStats.OnShipUpdated -= UpdateShip;
    }

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

                if (nextScheduledTarget != null)
                {
                    Vector2Int target = (Vector2Int)nextScheduledTarget;
                    Vector2Int currentShipCoords = gameMapHandler.shipCoordinates;
                    gameMapHandler.SetDestination(target);
                    shipStartPosition = transform.position;
                    shipTargetPosition = gameMapHandler.GetCellPosition(target);

                    demoController.SetGameState(EnumGameStates.ShipMoving);
                    shipMovingTimer = 1f;

                    CustomTile targetTile = gameMapHandler.GetMapTile(target);
                    shipStats.direction = gameMapHandler.GetDirection(currentShipCoords, target);
                    shipStats.NextTurn(targetTile);
                    demoController.FieldsTravelled++;

                    nextScheduledTarget = null;
                }
            }
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

                if (Input.GetMouseButtonUp(0))
                {
                    Vector2Int currentShipCoords = gameMapHandler.shipCoordinates;

                    if (gameMapHandler.CanNavigate(mouseCellCoordinates))
                    {
                        // check for pirates
                        if (gameMapHandler.PiratesPresent(mouseCellCoordinates) != null)
                        {
                            DemoController.Instance.shipBattleManager.StartShipBattle(shipStats, gameMapHandler.PiratesPresent(mouseCellCoordinates));
                        }
                        else
                        {
                            if (demoController.GameState == EnumGameStates.ShipMoving)
                            {
                                if (nextScheduledTarget == null)
                                {
                                    nextScheduledTarget = mouseCellCoordinates;
                                }
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
                        gameMapHandler.CoinIcon.gameObject.SetActive(false);
                        transform.position = gameMapHandler.GetCellPosition(mouseCellCoordinates); 
                        tradeController.ShowTrade(gameMapHandler.GetCellPosition(mouseCellCoordinates), StaticTileDataContainer.Instance.gameMapData[mouseCellCoordinates.x, mouseCellCoordinates.y].CityData);
                    }
                    else
                    {
                        Debug.Log("Can't navigate or trade here");
                    }
                }
            }
        }
    }

    private void EndShipMovement()
    {
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

    void UpdateShip(ShipStats stats)
    {
        //if (doShipRotate)
        //{
        //    if (demoController.GameState == EnumGameStates.ShipLost)
        //    {

        //        shipSpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);  // probably not needed
        //    }
        //    else
        //    {
        //        float rotation = 0;
        //        shipSpriteRenderer.flipX = false;

        //        switch (shipStats.direction)
        //        {
        //            case Direction.North:
        //                Debug.Log("North");
        //                rotation = -90;
        //                break;
        //            case Direction.NorthEast:
        //                Debug.Log("NorthEast");
        //                shipSpriteRenderer.flipX = true;
        //                rotation = 30;
        //                break;
        //            case Direction.NorthWest:
        //                Debug.Log("NorthWest");
        //                rotation = -30;
        //                break;
        //            case Direction.South:
        //                Debug.Log("South");
        //                rotation = 90;
        //                break;
        //            case Direction.SouthEast:
        //                Debug.Log("SouthEast");
        //                shipSpriteRenderer.flipX = true;
        //                rotation = -30;
        //                break;
        //            case Direction.SouthWest:
        //                Debug.Log("SouthWest");
        //                rotation = 30;
        //                break;
        //        }

        //        shipSpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, rotation);
        //    }
        //}
    }

    internal void ResetShipPosition()
    {
        transform.position = shipTargetPosition;
    }
}

