using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShipStats : MonoBehaviour
{
    [HideInInspector]
    public float ShipDurability; // 
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

    public Direction? direction;

    public delegate void ShipUpdated(ShipStats stats);
    public static event ShipUpdated OnShipUpdated;

    public void SetShip(ShipModelSO newShipModel)
    {
        this.shipModel = newShipModel;
        // shipVisual.ShowShipidle();

        ShipDurability = shipModel.ShipDurabilityMax;
        CrewCount = (int)Mathf.Ceil(shipModel.CrewCountMax / 2);
        FoodStatus = shipModel.FoodStatusMax;
        MoralStatus = shipModel.MoralStatusMax;

        OnShipUpdated?.Invoke(this);
    }

    public void NextTurn(CustomTile newTile)
    {
        float partOfDay = 1f / shipModel.ShipSpeed;

        ShipDurability -= shipDamagePerDayfromSailing * partOfDay;
        FoodStatus -= foodPerCrewPerDay * partOfDay;

        if (newTile.type.Equals(EnumTileType.DeepSea))
        {
            MoralStatus -= moralLossOnSea * partOfDay;
        }
        else if (newTile.type.Equals(EnumTileType.CoastalWater))
        {
            MoralStatus -= moralLossAtCoast * partOfDay;
        }


        if (ShipDurability < 0 || CrewCount < 0 || MoralStatus < 0 || FoodStatus < 0)
        {
            DemoController.Instance.SetGameState(EnumGameStates.ShipLost);
        }

        OnShipUpdated?.Invoke(this);
    }

    public void TriggerShipUpdated()
    {
        OnShipUpdated?.Invoke(this);
    }

    internal void AddStatsModifier(RandomEventResult randomEventResult)
    {
        float randomValue = Random.Range(randomEventResult.valueMin, randomEventResult.valueMax);

        if (randomEventResult.type.Equals(EnumShipStatModifierType.Durability))
        {
            AddShipStatus(randomValue);
            Debug.Log("Added " + randomValue + " Durability");
        }
        else if (randomEventResult.type.Equals(EnumShipStatModifierType.Crew))
        {
            AddShipCrew(randomValue);
            Debug.Log("Added " + randomValue + " Crew");
        }
        else if (randomEventResult.type.Equals(EnumShipStatModifierType.Food))
        {
            AddShipFood(randomValue);
            Debug.Log("Added " + randomValue + " Food");
        }
        else if (randomEventResult.type.Equals(EnumShipStatModifierType.Moral))
        {
            AddShipMoral(randomValue);
            Debug.Log("Added " + randomValue + " Moral");
        }
        else if (randomEventResult.type.Equals(EnumShipStatModifierType.Gold))
        {
            AddGold(randomValue);
            Debug.Log("Added " + randomValue + " Gold");
        }

        TriggerShipUpdated();
    }

    private void AddShipStatus(float randomValue)
    {
        ShipDurability = Mathf.Min(ShipDurability + randomValue, shipModel.ShipDurabilityMax);
    }

    private void AddShipCrew(float randomValue)
    {
        CrewCount = Mathf.Min(CrewCount + Mathf.FloorToInt(randomValue), shipModel.CrewCountMax);
    }

    private void AddShipFood(float randomValue)
    {
        FoodStatus = Mathf.Min(FoodStatus + randomValue, shipModel.FoodStatusMax);
    }

    private void AddShipMoral(float randomValue)
    {
        MoralStatus = Mathf.Min(MoralStatus + randomValue, shipModel.MoralStatusMax);
    }

    private void AddGold(float randomValue)
    {
        Gold = Gold + Mathf.FloorToInt(randomValue);
    }
}
