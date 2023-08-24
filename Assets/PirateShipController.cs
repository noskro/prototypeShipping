using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateShipController : MonoBehaviour
{
    public GameMapHandler gameMapHandler;
    public ShipStats shipStats;
    private SpriteRenderer shipSpriteRenderer;

    public Vector2Int pirateShipCoordinates;

    public EnumGameStates pirateShipState;

    // Start is called before the first frame update
    void Start()
    {
        shipStats = GetComponent<ShipStats>();
        shipSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void PerformAction()
    {
        if (pirateShipState != EnumGameStates.ShipLost)
        {
            Vector2Int targetCell = new Vector2Int(-1, -1);

            // check if enemy in range
            List<Vector2Int> detectionRange = DemoController.Instance.gameMapHandler.GetNeighbors(pirateShipCoordinates, 2);
            if (detectionRange.Contains(DemoController.Instance.gameMapHandler.shipCoordinates))
            {
                // ship in detection range    

                if (DemoController.Instance.gameMapHandler.CanNavigate(DemoController.Instance.gameMapHandler.shipCoordinates, pirateShipCoordinates))
                {
                    // can attack
                    targetCell = DemoController.Instance.gameMapHandler.shipCoordinates;
                }
                else
                {
                    // select random target for now
                }
            }

            // else get random direction for ship movement
            if (targetCell == new Vector2Int(-1, -1))
            {
                List<Vector2Int> possibleTargets = DemoController.Instance.gameMapHandler.GetNeighbors(pirateShipCoordinates, 1);

                while (possibleTargets.Count > 0)
                {
                    int randomTarget = Random.Range(0, possibleTargets.Count);
                    targetCell = possibleTargets[randomTarget];
                    if (!DemoController.Instance.gameMapHandler.CanNavigate(targetCell, pirateShipCoordinates)) // check if the pirate ship can move the 1 space to the target
                    {
                        possibleTargets.RemoveAt(randomTarget);
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (targetCell != new Vector2Int(-1, -1))
            {
                // attack or move
                if (targetCell == DemoController.Instance.gameMapHandler.shipCoordinates)
                {
                    // attack
                    DemoController.Instance.shipBattleManager.CalculateFight(shipStats, DemoController.Instance.shipController.shipStats);
                }
                else
                {
                    // move
                    Vector3 shipTargetPosition = gameMapHandler.GetCellPosition(targetCell);
                    transform.position = shipTargetPosition; // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);

                    shipStats.direction = gameMapHandler.GetDirection(pirateShipCoordinates, targetCell);
                    pirateShipCoordinates = targetCell;

                    //gameMapHandler.HandleRandomEvents(gameMapHandler.shipCoordinates); // maybe later
                }
            }
            // else no possible target found
        }

        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (shipSpriteRenderer != null && shipStats != null)
        {
            if (pirateShipState == EnumGameStates.ShipLost)
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
