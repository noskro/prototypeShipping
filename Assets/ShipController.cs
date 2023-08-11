using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipController : MonoBehaviour
{
    public Tilemap tilemap_World;
    public GameMapHandler gameMapHandler;
    public ShipStats shipStats;
    public SpriteRenderer shipSpriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        shipStats = GetComponent<ShipStats>();
        shipSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<ShipStats>().shipLost)
        {
            shipSpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);
            gameMapHandler.ShowMouseCursor(new Vector3Int(-1, -1, -1));

        }
        else
        {
            Vector3 mousePositin = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3Int coordinates = tilemap_World.WorldToCell(mousePositin);

            if (coordinates != null && coordinates.x >= 0 && coordinates.y >= 0)
            {
                coordinates.z = 0;
                gameMapHandler.ShowMouseCursor(coordinates);

                if (!gameMapHandler.IsShipMoving && !gameMapHandler.tradeController.IsTrading && Input.GetMouseButton(0))
                {
                    Vector3Int previousShipCoordinates = gameMapHandler.shipCoordinates;

                    var direction = gameMapHandler.GetDirection(previousShipCoordinates, coordinates);

                    float rotation = 0;
                    shipSpriteRenderer.flipX = false;

                    switch (direction)
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

                    CustomTile targetTile = gameMapHandler.ClickOnCoords(coordinates);
                    if (targetTile != null) // if ship was moved)
                    {
                        shipSpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, rotation);

                        shipStats.NextTurn(targetTile);
                    }
                }
            }
        }
    }
}

