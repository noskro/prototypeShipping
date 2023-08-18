using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipController : MonoBehaviour
{
    public GameMapHandler gameMapHandler;
    private ShipStats shipStats;
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
            shipMovingTimer -= Time.deltaTime * 2;

            if (shipMovingTimer <= 0)
            {
                // ship.GetComponent<ShipVisual>().ShowShipMoving(false);

                transform.position = shipTargetPosition; // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);

                gameMapHandler.DiscoverNewAreaByShip(gameMapHandler.shipCoordinates, DemoController.Instance.currentShipModel);

                gameMapHandler.UpdateBeacons(gameMapHandler.shipCoordinates);

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
        else if (demoController.GameState == EnumGameStates.ShipMoving)
        {
            // no game input while ship is moving
        }
        else if (tradeController.IsTrading) // maybe handle this in DemoController instead?
        {
            // handled by tradeController
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
                    }
                    else if (gameMapHandler.CanTrade(mouseCellCoordinates))
                    {
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
                    case GameMapHandler.Direction.North:
                        Debug.Log("North");
                        rotation = -90;
                        break;
                    case GameMapHandler.Direction.NorthEast:
                        Debug.Log("NorthEast");
                        shipSpriteRenderer.flipX = true;
                        rotation = 30;
                        break;
                    case GameMapHandler.Direction.NorthWest:
                        Debug.Log("NorthWest");
                        rotation = -30;
                        break;
                    case GameMapHandler.Direction.South:
                        Debug.Log("South");
                        rotation = 90;
                        break;
                    case GameMapHandler.Direction.SouthEast:
                        Debug.Log("SouthEast");
                        shipSpriteRenderer.flipX = true;
                        rotation = -30;
                        break;
                    case GameMapHandler.Direction.SouthWest:
                        Debug.Log("SouthWest");
                        rotation = 30;
                        break;
                }

                shipSpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, rotation);

            }
        }

    }
}

