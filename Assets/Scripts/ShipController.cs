using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipController : MonoBehaviour
{
    public float ShipMovementAnimationSpeed;

    public GameMapHandler gameMapHandler;
    public ShipStats shipStats;
    private SpriteRenderer shipSpriteRenderer;
    private TradeController tradeController;
    private DemoController demoController;
    public Vector2Int shipCoordinates;

    public Vector3 shipStartPosition;
    public Vector3 shipTargetPosition;
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
                // ship.GetComponent<ShipVisual>().ShowShipMoving(false);

                transform.position = shipTargetPosition; // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);

                gameMapHandler.DiscoverNewAreaByShip(gameMapHandler.shipCoordinates, this.shipStats);

                //gameMapHandler.UpdateBeacons(gameMapHandler.shipCoordinates);

                // handle random events               
                gameMapHandler.HandleRandomEvents(gameMapHandler.shipCoordinates);

                demoController.SetGameState(EnumGameStates.ShipIdle);
            }
        }
    }

    void HandleInput()
    {
        if (demoController.GameState == EnumGameStates.ShipLost)
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

                if ((Input.GetMouseButton(0) && demoController.GameState != EnumGameStates.ShipMoving) ||
                    (Input.GetMouseButtonDown(0) && demoController.GameState == EnumGameStates.ShipMoving))
                {
                    if (demoController.GameState == EnumGameStates.ShipMoving) // TEST: moving ship can be skipped for next movement 
                    {
                        demoController.GameState = EnumGameStates.ShipIdle;
                        shipMovingTimer = 0;
                        transform.position = shipTargetPosition;
                        gameMapHandler.DiscoverNewAreaByShip(gameMapHandler.shipCoordinates, this.shipStats);
                        gameMapHandler.HandleRandomEvents(gameMapHandler.shipCoordinates);
                    }

                    Vector2Int currentShipCoords = gameMapHandler.shipCoordinates;

                    if (gameMapHandler.CanNavigate(mouseCellCoordinates))
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

    void UpdateShip(ShipStats stats)
    {
        if (doShipRotate)
        {
            if (demoController.GameState == EnumGameStates.ShipLost)
            {

                shipSpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);  // probably not needed
            }
            else
            {
                float rotation = 0;
                shipSpriteRenderer.flipX = false;

                switch (shipStats.direction)
                {
                    case Direction.North:
                        Debug.Log("North");
                        rotation = -90;
                        break;
                    case Direction.NorthEast:
                        Debug.Log("NorthEast");
                        shipSpriteRenderer.flipX = true;
                        rotation = 30;
                        break;
                    case Direction.NorthWest:
                        Debug.Log("NorthWest");
                        rotation = -30;
                        break;
                    case Direction.South:
                        Debug.Log("South");
                        rotation = 90;
                        break;
                    case Direction.SouthEast:
                        Debug.Log("SouthEast");
                        shipSpriteRenderer.flipX = true;
                        rotation = -30;
                        break;
                    case Direction.SouthWest:
                        Debug.Log("SouthWest");
                        rotation = 30;
                        break;
                }

                shipSpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, rotation);

            }
        }

    }

    internal void ResetShipPosition()
    {
        transform.position = shipTargetPosition;
    }
}

