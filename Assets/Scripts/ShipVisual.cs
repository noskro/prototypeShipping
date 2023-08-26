using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ShipVisual : MonoBehaviour
{
    private SpriteRenderer shipSpriteRenderer;

    //public Sprite shipIdle;
    //public Sprite shipMoving;
    //public Sprite shipLost;

    //private ShipModelSO _ship;
    private ShipStats shipStats;
    private EnumGameStates state;

    void Start()
    {
        //_ship = shipStats.shipModel; // DemoController.Instance.currentShipModel;
        DemoController.OnGameStateChanged += (state) =>
        {
            this.state = state;
            UpdateSprite();
        };
        ShipController.OnShipUpdated += (stats) =>
        {
            shipStats = stats;
            UpdateSprite();
        };

        shipSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //shipSpriteRenderer.sprite = _ship.ShipSpriteIdle;

    }

    private void UpdateSprite()
    {
        if (shipSpriteRenderer != null && shipStats != null)
        {
            if (state == EnumGameStates.ShipLost)
            {
                shipSpriteRenderer.sprite = shipStats.shipModel.ShipSpriteLost;
            }
            else
            {
                shipSpriteRenderer.sprite = shipStats.direction switch
                {
                    Direction.North => shipStats.shipModel.ShipSpriteMovingNorth,
                    Direction.NorthEast => shipStats.shipModel.ShipSpriteMovingNorthEast,
                    Direction.NorthWest => shipStats.shipModel.ShipSpriteMovingNorthWest,
                    Direction.South => shipStats.shipModel.ShipSpriteMovingSouth,
                    Direction.SouthEast => shipStats.shipModel.ShipSpriteMovingSouthEast,
                    Direction.SouthWest => shipStats.shipModel.ShipSpriteMovingSouthWest,
                    _ => shipStats.shipModel.ShipSpriteIdle,
                };
            }
        }
    }
}
