﻿using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ShipModel", order = 1)]
public class ShipModelSO : ScriptableObject
{
    public string ShipModelName;
    public int ShipDurabilityMax; // how much damage the ship can take
    public int CrewCountMax;
    public int FoodStatusMax;
    public int MoralStatusMax; // moral is maybe not dependent on ship? maybe upgrades, like a pool on board or a gaming room or so :D

    public int ShipSpeed; // amount of fields a ship can move at one virtual "day"

    public int ShipPrice; // value of ship to unlock/sell or something like that

    public int ViewRange;
    public int DiscoverRange;

    public Sprite ShipSpriteIdle;
    public Sprite ShipSpriteLost;

    public Sprite ShipSpriteMovingNorth;
    public Sprite ShipSpriteMovingNorthEast;
    public Sprite ShipSpriteMovingSouthEast;
    public Sprite ShipSpriteMovingSouth;
    public Sprite ShipSpriteMovingSouthWest;
    public Sprite ShipSpriteMovingNorthWest;

}