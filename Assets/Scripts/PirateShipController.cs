using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateShipController : MonoBehaviour
{
    public GameMapHandler gameMapHandler;
    public ShipStats shipStats;
    private SpriteRenderer shipSpriteRenderer;

    public Vector2Int pirateShipCoordinatesStarting;
    public Vector2Int pirateShipCoordinates;

    public EnumGameStates pirateShipState;

    // Start is called before the first frame update
    void Start()
    {
        shipStats = GetComponent<ShipStats>();
        shipSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ResetForNewRun();
    }

    public void PerformAction()
    {
        DemoController.Instance.shipController.shipStatusUI.ShowPossibleStatChangeString(null, null, null, null, null, null);

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
                    // select a field to follow player
                    List<Vector2Int> neighboursOfPlayer = DemoController.Instance.gameMapHandler.GetNeighbors(DemoController.Instance.gameMapHandler.shipCoordinates, 1);
                    foreach (Vector2Int target in neighboursOfPlayer)
                    {
                        if (DemoController.Instance.gameMapHandler.CanNavigate(pirateShipCoordinates, target))
                        {
                            targetCell = target;
                            break;
                        }
                    }
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
                    DemoController.Instance.shipBattleManager.StartShipBattle(shipStats, DemoController.Instance.shipController.shipStats);
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

    internal void ResetForNewRun()
    {
        this.gameObject.SetActive(true);

        if (this.shipStats != null)
        {
            this.shipStats.SetShipModel(shipStats.shipModel);

            pirateShipCoordinates = pirateShipCoordinatesStarting;
            Vector3 shipTargetPosition = gameMapHandler.GetCellPosition(pirateShipCoordinates);
            transform.position = shipTargetPosition; // new Vector3(shipWorldPosition.x, shipWorldPosition.y, -10);

            shipStats.direction = Direction.SouthEast;
            pirateShipState = EnumGameStates.ShipIdle;
            UpdateSprite();
        }
    }

    public void UpdateSprite()
    {
        if (shipSpriteRenderer != null && shipStats != null)
        {
            if (pirateShipState == EnumGameStates.ShipLost)
            {
                shipSpriteRenderer.sprite = null; // shipStats.shipModel.ShipSpriteLost;
                this.gameObject.SetActive(false);
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

    internal void SetPirateShipLost()
    {
        pirateShipState = EnumGameStates.ShipLost;
        UpdateSprite();

        int randomID = Random.Range(0, DemoController.Instance.pirateDefeatEventList.Count);
        RandomMapEventSO pirateDefeatresult = DemoController.Instance.pirateDefeatEventList[randomID];
        StaticTileDataContainer.Instance.TilemapObjects.SetTile(new Vector3Int(pirateShipCoordinates.x, pirateShipCoordinates.y, 0), pirateDefeatresult.eventTile);
    }
}
