using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ShipStats : MonoBehaviour
{
    [HideInInspector]
    public float ShipStatus; // 
    [HideInInspector]
    public int CrewCount;
    [HideInInspector]
    public float FoodStatus;
    [HideInInspector]
    public float MoralStatus;
    [HideInInspector]
    public int Gold;

    public ShipModelSO shipModel;

    [Header("Settings")]
    public float shipDamagePerDayfromSailing;
    public float foodPerCrewPerDay;
    public float moralLossOnSea;
    public float moralLossAtCoast;

    [HideInInspector]
    public bool shipLost = true;

    public GameMapHandler.Direction? direction;

    public delegate void ShipUpdated();
    public static event ShipUpdated OnShipUpdated;

    public void SetShip(ShipModelSO newShipModel)
    {
        this.shipModel = newShipModel;
        // shipVisual.ShowShipidle();

        ShipStatus = shipModel.ShipStatusMax;
        CrewCount = (int)Mathf.Ceil(shipModel.CrewCountMax / 2);
        FoodStatus = shipModel.FoodStatusMax;
        MoralStatus = shipModel.MoralStatusMax;

        shipLost = false;

        OnShipUpdated?.Invoke();
    }

    public void NextTurn(CustomTile newTile)
    {
        float partOfDay = 1f / shipModel.ShipSpeed;

        ShipStatus -= shipDamagePerDayfromSailing * partOfDay;
        FoodStatus -= foodPerCrewPerDay * partOfDay;

        if (newTile.type.Equals(EnumTileType.DeepSea))
        {
            MoralStatus -= moralLossOnSea * partOfDay;
        }
        else if (newTile.type.Equals(EnumTileType.CoastalWater))
        {
            MoralStatus -= moralLossAtCoast * partOfDay;
        }


        if (ShipStatus < 0 || CrewCount < 0 || MoralStatus < 0 || FoodStatus < 0)
        {
            shipLost = true;
        }

        OnShipUpdated?.Invoke();
    }
}
