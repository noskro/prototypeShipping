using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ShipVisual : MonoBehaviour
{
    private SpriteRenderer shipSpriteRenderer;

    //public Sprite shipIdle;
    //public Sprite shipMoving;
    //public Sprite shipLost;

    public ShipModelSO ship;
    private ShipStats shipStats;
    private DemoController.GameStates state;

    void Start()
    {
        ship = DemoController.Instance.demoShipModel;
        DemoController.OnGameStateChanged += (state) =>
        {
            this.state = state;
            UpdateSprite();
        };
        ShipStats.OnShipUpdated += (stats) =>
        {
            shipStats = stats;
            UpdateSprite();
        };

        shipSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        shipSpriteRenderer.sprite = ship.ShipSpriteIdle;

    }

    private void UpdateSprite()
    {
        if (state == DemoController.GameStates.ShipLost)
        {
            shipSpriteRenderer.sprite = ship.ShipSpriteLost;
        }
        else
        {
            shipSpriteRenderer.sprite = shipStats.direction switch
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
}
