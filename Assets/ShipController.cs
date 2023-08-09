using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipController : MonoBehaviour
{
    public Tilemap tilemap_World;
    public GameMapHandler gameMapHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePositin = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3Int coordinates = tilemap_World.WorldToCell(mousePositin);

        if (coordinates != null && coordinates.x >= 0 && coordinates.y >= 0)
        {
            coordinates.z = 0;
            gameMapHandler.ShowMouseCursor(coordinates);

            if (!gameMapHandler.IsShipMooving && Input.GetMouseButton(0))
            {
                gameMapHandler.MoveShipTocoords(coordinates);
            }
        }
    }
}

