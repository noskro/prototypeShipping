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

    private int shipDurabilityCurrentLevel;
    private int shipMaxCrewCurrentLevel;
    private int shipMaxFoodCurrentLevel;
    private int shipMaxMoralCurrentLevel;
    private int shipMaxCanonsCurrentLevel;
    private int shipSpeedCurrentLevel;
    private int shipViewRangeCurrentLevel;
    private int shipDiscoverRangeCurrentLevel;

    public ShipModelSO shipModel;

    [Header("Settings")]
    public float shipDamagePerDayfromSailing;
    public float foodPerCrewPerDay;
    public float moralLossOnSea;
    public float moralLossAtCoast;

    public Direction? direction;

    public delegate void ShipUpdated(ShipStats stats);
    public static event ShipUpdated OnShipUpdated;

    private void Start()
    {
        shipDurabilityCurrentLevel = 0;
        shipMaxCrewCurrentLevel = 0;
        shipMaxFoodCurrentLevel = 0;
        shipMaxMoralCurrentLevel = 0;
        shipMaxCanonsCurrentLevel = 0;
        shipSpeedCurrentLevel = 0;
        shipViewRangeCurrentLevel = 0;
        shipDiscoverRangeCurrentLevel = 0;
    }

    public void SetShipModel(ShipModelSO newShipModel)
    {
        this.shipModel = newShipModel;
        // shipVisual.ShowShipidle();


        ShipDurability = shipModel.ShipDurabilityUpgradeList[shipDurabilityCurrentLevel].Value;
        CrewCount = (int)Mathf.Ceil(shipModel.ShipMaxCrewUpgradeList[shipMaxCrewCurrentLevel].Value / 2);
        FoodStatus = shipModel.ShipMaxFoodUpgradeList[shipMaxFoodCurrentLevel].Value;
        MoralStatus = shipModel.ShipMaxMoralUpgradeList[shipMaxMoralCurrentLevel].Value;

        OnShipUpdated?.Invoke(this);
    }

    public void NextTurn(CustomTile newTile)
    {
        float partOfDay = 1f / shipModel.ShipSpeedUpgradeList[shipMaxCanonsCurrentLevel].Value;
            
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
        ShipDurability = Mathf.Min(ShipDurability + randomValue, shipModel.ShipDurabilityUpgradeList[shipDurabilityCurrentLevel].Value);
    }

    private void AddShipCrew(float randomValue)
    {
        CrewCount = Mathf.Min(CrewCount + Mathf.FloorToInt(randomValue), shipModel.ShipMaxCrewUpgradeList[shipMaxCrewCurrentLevel].Value);
    }

    private void AddShipFood(float randomValue)
    {
        FoodStatus = Mathf.Min(FoodStatus + randomValue, shipModel.ShipMaxFoodUpgradeList[shipMaxFoodCurrentLevel].Value);
    }

    private void AddShipMoral(float randomValue)
    {
        MoralStatus = Mathf.Min(MoralStatus + randomValue, shipModel.ShipMaxMoralUpgradeList[shipMaxMoralCurrentLevel].Value);
    }

    private void AddGold(float randomValue)
    {
        Gold = Gold + Mathf.FloorToInt(randomValue);
    }

    internal int GetUpgradeableSingleStatCurrent(EnumShipStatModifierType shipStatModifier)
    {
        if (shipStatModifier.Equals(EnumShipStatModifierType.Durability))
        {
            return shipDurabilityCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Crew))
        {
            return shipMaxCrewCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Food))
        {
            return shipMaxFoodCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Moral))
        {
            return shipMaxMoralCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Canons))
        {
            return shipMaxCanonsCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.ViewRange))
        {
            return shipViewRangeCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.DiscoverRange))
        {
            return shipDiscoverRangeCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Speed))
        {
            return shipSpeedCurrentLevel; ;
        }
        return -1;
    }

    internal List<ShipSingleStatUpgadeItem> GetUpgradeableSingleStatList(EnumShipStatModifierType shipStatModifier)
    {
        if (shipStatModifier.Equals(EnumShipStatModifierType.Durability))
        {
            return this.shipModel.ShipDurabilityUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Crew))
        {
            return this.shipModel.ShipMaxCrewUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Food))
        {
            return this.shipModel.ShipMaxFoodUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Moral))
        {
            return this.shipModel.ShipMaxMoralUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Canons))
        {
            return this.shipModel.ShipMaxCanonsUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.ViewRange))
        {
            return this.shipModel.ShipViewRangeUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.DiscoverRange))
        {
            return this.shipModel.ShipDiscoverRangeUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Speed))
        {
            return this.shipModel.ShipSpeedUpgradeList;
        }
        return null;
    }

    internal void SetUpgradeableSingleStat(EnumShipStatModifierType shipStatModifier, int value)
    {
        if (shipStatModifier.Equals(EnumShipStatModifierType.Durability))
        {
            shipDurabilityCurrentLevel = value;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Crew))
        {
            shipMaxCrewCurrentLevel = value;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Food))
        {
            shipMaxFoodCurrentLevel = value;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Moral))
        {
            shipMaxMoralCurrentLevel = value;
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Canons))
        {
            shipMaxCanonsCurrentLevel= value;   
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.ViewRange))
        {
            shipViewRangeCurrentLevel= value;   
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.DiscoverRange))
        {
            shipDiscoverRangeCurrentLevel= value;   
        }
        else if (shipStatModifier.Equals(EnumShipStatModifierType.Speed))
        {
            shipSpeedCurrentLevel= value;   
        }
    }

    internal int GetCurrentDiscoverRange()
    {
        return shipModel.ShipDiscoverRangeUpgradeList[shipDiscoverRangeCurrentLevel].Value;
    }

    internal int GetCurrentViewRange()
    {
        return shipModel.ShipViewRangeUpgradeList[shipViewRangeCurrentLevel].Value;
    }

    internal int GetCurrentMaxDurability()
    {
        return shipModel.ShipDurabilityUpgradeList[shipDurabilityCurrentLevel].Value;
    }

    internal int GetCurrentMaxCrew()
    {
        return shipModel.ShipMaxCrewUpgradeList[shipMaxCrewCurrentLevel].Value;
    }

    internal int GetCurrentMaxFood()
    {
        return shipModel.ShipMaxFoodUpgradeList[shipMaxFoodCurrentLevel].Value;
    }

    internal int GetCurrentMaxMoral()
    {
        return shipModel.ShipMaxMoralUpgradeList[shipMaxMoralCurrentLevel].Value;
    }

    internal int GetCurrentMaxCanons()
    {
        return shipModel.ShipMaxCanonsUpgradeList[shipMaxCanonsCurrentLevel].Value;
    }
}
