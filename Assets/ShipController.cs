using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipController : MonoBehaviour
{
    public Tilemap tilemap_World;
    public GameMapHandler gameMapHandler;
    private ShipStats shipStats;
    private SpriteRenderer shipSpriteRenderer;

    public bool doShipRotate;

    // Start is called before the first frame update
    void Start()
    {
        shipStats = GetComponent<ShipStats>();
        shipSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
    }

    void HandleInput()
    {
        if (shipStats.shipLost)
        {
            // no input while ship is lost
        }
        else if (gameMapHandler.IsShipMoving)
        {
            // no input while ship is moving
        }
        else if (gameMapHandler.tradeController.IsTrading)
        {
            // handled by tradeController
        }
        else
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mouseCellCoordinates = tilemap_World.WorldToCell(mouseWorldPosition);

            if (mouseCellCoordinates != null && mouseCellCoordinates.x >= 0 && mouseCellCoordinates.y >= 0)
            {
                mouseCellCoordinates.z = 0;
                gameMapHandler.ShowMouseCursor(mouseCellCoordinates);

                if (Input.GetMouseButton(0))
                {
                    Vector3Int previousShipCoordinates = gameMapHandler.shipCoordinates;

                    shipStats.direction = gameMapHandler.GetDirection(previousShipCoordinates, mouseCellCoordinates);

                    CustomTile targetTile = gameMapHandler.ClickOnCoords(mouseCellCoordinates);
                    if (targetTile != null) // if ship was moved)
                    {
                        shipStats.direction = gameMapHandler.GetDirection(currentShipCoords, mouseCellCoordinates);
                        shipStats.NextTurn(targetTile);
                    }
                }
            }
        }

    }

    void UpdateShip()
    {
        if (doShipRotate)
        {
        if (shipStats.shipLost)
        {

            shipSpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);
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

