using System.Collections;
using System.Collections.Generic;
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
        if (!GetComponent<ShipStats>().shipLost)
        {
            Vector3 mousePositin = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3Int coordinates = tilemap_World.WorldToCell(mousePositin);

            if (coordinates != null && coordinates.x >= 0 && coordinates.y >= 0)
            {
                coordinates.z = 0;
                gameMapHandler.ShowMouseCursor(coordinates);

                if (!gameMapHandler.IsShipMooving && !gameMapHandler.tradeController.IsTrading && Input.GetMouseButton(0))
                {
                    Vector3Int previousShipCoordinates = gameMapHandler.shipCoordinates;

                    CustomTile targetTile = gameMapHandler.ClickOnCoords(coordinates);

                    if (targetTile != null) // if ship was moved)
                    {
                        float rotation = 0;
                        shipSpriteRenderer.transform.localScale = new Vector3(1, 1, 1);

                        if (coordinates.y > previousShipCoordinates.y)
                        {
                            shipSpriteRenderer.transform.localScale = new Vector3(-1, 1, 1);

                            if (coordinates.x < previousShipCoordinates.x)
                            {
                                rotation = -30;
                            }
                            else if (coordinates.x > previousShipCoordinates.x)
                            {
                                rotation = 30;
                            }
                        }
                        else if (coordinates.y < previousShipCoordinates.y)
                        {
                            if (coordinates.x < previousShipCoordinates.x)
                            {
                                rotation = 30;
                            }
                            else if (coordinates.x > previousShipCoordinates.x)
                            {
                                rotation = -30;
                            }
                        }
                        else
                        {
                            if (coordinates.x < previousShipCoordinates.x)
                            {
                                rotation = 90;
                            }
                            else if (coordinates.x > previousShipCoordinates.x)
                            {
                                rotation = -90;
                            }
                        }

                        shipSpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, rotation);

                        shipStats.NextTurn(targetTile);
                    }
                }
            }
        }
        else // ship lost
        {
            shipSpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);
            gameMapHandler.ShowMouseCursor(new Vector3Int(-1,-1,-1));
        }
    }
}

