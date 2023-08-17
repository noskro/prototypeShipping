using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ArtefactBeacon : MonoBehaviour
{
    public Vector2Int CenterPoint;

    public Vector2Int shipcoordinates;

    private float DistanceToShip;
    private bool beaconActive = false;
    private float beaconPulseCounter = 0f;

    public void SetShipCoordinates(Tilemap map, Vector2Int ship)
    {
        this.shipcoordinates = ship;

        Vector3 shipWorld = map.CellToWorld(new Vector3Int(ship.x, ship.y, 0));
        Vector3 beaconWorld = map.CellToWorld(new Vector3Int(CenterPoint.x, CenterPoint.y, 0));

        DistanceToShip = 30f; // Vector3.Distance(shipWorld, beaconWorld); // 
        
        this.beaconActive = true;
    }
        

    // Update is called once per frame
    void Update()
    {
        if (beaconActive)
        { 
            if (beaconPulseCounter <= 1f)
            {                
                this.transform.localScale = new Vector3(1, 1, 1) + Vector3.Lerp(new Vector3(0, 0, 1), new Vector3(DistanceToShip, DistanceToShip * 0.6f, 1), Mathf.Abs(beaconPulseCounter));
                beaconPulseCounter += Time.deltaTime;
            }
            if (beaconPulseCounter > 1f)
            {
                beaconPulseCounter = -1f;
            }
        }
    }

    internal void PlaceBeacon(Vector2Int center, Vector3 pos)
    {
        this.CenterPoint = center;
        this.transform.position = pos;
    }
}
