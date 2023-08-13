using UnityEngine;

public class ShipVisual : MonoBehaviour
{
    private SpriteRenderer shipSpriteRenderer;

    public Sprite shipIdle;
    public Sprite shipMoving;
    public Sprite shipLost;

    public ShipModelSO ship;

    // Start is called before the first frame update
    void Start()
    {
        ship = DemoController.Instance.demoShipModel;

        shipSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        shipSpriteRenderer.sprite = ship.ShipSpriteIdle;
    }

    private void Update()
    {
        var shipstats = GetComponent<ShipStats>();
        if (shipstats.shipLost)
        {
            shipSpriteRenderer.sprite = ship.ShipSpriteLost;
        }
        else
        {
            shipSpriteRenderer.sprite = shipstats.direction switch
            {
                GameMapHandler.Direction.North => ship.ShipSpriteMovingNorth,
                GameMapHandler.Direction.NorthEast => ship.ShipSpriteMovingNorthEast,
                GameMapHandler.Direction.NorthWest => ship.ShipSpriteMovingNorthWest,
                GameMapHandler.Direction.South => ship.ShipSpriteMovingSouth,
                GameMapHandler.Direction.SouthEast => ship.ShipSpriteMovingSouthEast,
                GameMapHandler.Direction.SouthWest => ship.ShipSpriteMovingSouthWest,
                _ => ship.ShipSpriteIdle,
            };
        }
    }

    public void ShowShipidle()
    {
        shipSpriteRenderer.sprite = ship.ShipSpriteIdle;
    }

    public void ShowShipMoving(bool moving)
    {
        if (moving)
        {
            shipSpriteRenderer.sprite = shipMoving;
        }
        else
        {
            shipSpriteRenderer.sprite = shipIdle;
        }
    }
}
