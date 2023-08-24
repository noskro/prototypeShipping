using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateShipController : MonoBehaviour
{
    public GameMapHandler gameMapHandler;
    public ShipStats shipStats;
    private SpriteRenderer shipSpriteRenderer;

    public Vector2Int pirateShipCoordinates;

    // Start is called before the first frame update
    void Start()
    {
        shipStats = GetComponent<ShipStats>();
        shipSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void PerformAction()
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
                if (!DemoController.Instance.gameMapHandler.CanNavigate(targetCell, pirateShipCoordinates))
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
                DemoController.Instance.gameMapHandler.CalculateFight(shipStats, DemoController.Instance.shipController.shipStats);
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

    void UpdateShip(ShipStats stats)
    {
        if (DemoController.Instance.GameState == EnumGameStates.ShipLost)
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
