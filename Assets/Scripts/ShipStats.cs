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

    public SpriteRenderer spriteCanonFireSmoke;
    public SpriteRenderer spriteCanonFireHit;

    private int shipDurabilityCurrentLevel;
    private int shipMaxCrewCurrentLevel;
    private int shipMaxFoodCurrentLevel;
    private int shipMaxMoralCurrentLevel;
    private int shipMaxCanonsCurrentLevel;
    private int shipSpeedCurrentLevel;
    private int shipViewRangeCurrentLevel;
    private int shipDiscoverRangeCurrentLevel;

    private int temporaryViewRangeAddition = 0;
    private int temporaryCanonAddition = 0;

    public ShipModelSO shipModel;

    [Header("Settings")]
    public int shipDamageAtCoast;
    public int shipDamageAtSea;
    public float foodPerCrewPerDay;
    public float moralLossOnSea;
    public float moralLossAtCoast;

    public Direction? direction;


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

        if (shipModel != null)
        {
            SetShipModel(shipModel);
        }
    }

    public void SetShipModel(ShipModelSO newShipModel)
    {
        this.shipModel = newShipModel;

        ShipDurability = shipModel.ShipDurabilityUpgradeList[shipDurabilityCurrentLevel].Value;
        CrewCount = (int)Mathf.Ceil(shipModel.ShipMaxCrewUpgradeList[shipMaxCrewCurrentLevel].Value / 1); // max crew to start with
        FoodStatus = shipModel.ShipMaxFoodUpgradeList[shipMaxFoodCurrentLevel].Value;
        MoralStatus = shipModel.ShipMaxMoralUpgradeList[shipMaxMoralCurrentLevel].Value;
    }

    public void NextTurn(CustomTile newTile)
    {
        float partOfDay = 1f; //  / shipModel.ShipSpeedUpgradeList[shipSpeedCurrentLevel].Value;
            
        //FoodStatus -= foodPerCrewPerDay * partOfDay;

        if (newTile.type.Equals(EnumTileType.DeepSea))
        {
            //MoralStatus -= moralLossOnSea * partOfDay;
            ShipDurability -= shipDamageAtSea * partOfDay;
        }
        else if (newTile.type.Equals(EnumTileType.CoastalWater))
        {
            //MoralStatus -= moralLossAtCoast * partOfDay;
            ShipDurability -= shipDamageAtCoast * partOfDay;
        }


        if (ShipDurability < 0 || CrewCount < 0) // || MoralStatus < 0 || FoodStatus < 0)
        {
            DemoController.Instance.SetGameState(EnumGameStates.ShipLost);
        }
    }

    internal int getMaxAttackDamage()
    {
        return (Math.Min(this.GetCurrentCanons(), Mathf.FloorToInt(CrewCount/2)) * DemoController.Instance.DamagePerCanon);
    }

    internal void AddStatsModifier(RandomEventResult randomEventResult)
    {
        float randomValue = Random.Range(randomEventResult.valueMin, randomEventResult.valueMax);
        AddStatsModifier(randomEventResult.type, randomValue);
    }

    internal void AddStatsModifier(EnumEventModifierRewardType modifierType, float randomValue)
    {
        if (modifierType.Equals(EnumEventModifierRewardType.Durability))
        {
            AddShipDurability(randomValue);
            Debug.Log("Added " + randomValue + " Durability");
        }
        else if (modifierType.Equals(EnumEventModifierRewardType.Crew))
        {
            AddShipCrew(randomValue);
            Debug.Log("Added " + randomValue + " Crew");
        }
        else if (modifierType.Equals(EnumEventModifierRewardType.Food))
        {
            AddShipFood(randomValue);
            Debug.Log("Added " + randomValue + " Food");
        }
        else if (modifierType.Equals(EnumEventModifierRewardType.Moral))
        {
            AddShipMoral(randomValue);
            Debug.Log("Added " + randomValue + " Moral");
        }
        else if (modifierType.Equals(EnumEventModifierRewardType.Gold))
        {
            AddGold(randomValue);
            Debug.Log("Added " + randomValue + " Gold");
        }
        else if (modifierType.Equals(EnumEventModifierRewardType.ViewRange))
        {
            AddViewRange(randomValue);
        }
    }

    public void AddShipDurability(float randomValue)
    {
        int prevShipDurability = Mathf.FloorToInt(ShipDurability);
        ShipDurability = Mathf.Min(ShipDurability + randomValue, shipModel.ShipDurabilityUpgradeList[shipDurabilityCurrentLevel].Value);
        DemoController.Instance.shipController.shipStatusUI.ShowStatChange(Mathf.FloorToInt(ShipDurability)-prevShipDurability, 0, 0, 0, 0, 0);
    }

    public void AddShipCrew(float randomValue)
    {
        CrewCount = Mathf.Min(CrewCount + Mathf.FloorToInt(randomValue), shipModel.ShipMaxCrewUpgradeList[shipMaxCrewCurrentLevel].Value);
        DemoController.Instance.shipController.shipStatusUI.ShowStatChange(0, Mathf.FloorToInt(randomValue), 0, 0, 0, 0);
    }

    public void AddShipFood(float randomValue)
    {
        int prevFoodStatus = Mathf.FloorToInt(FoodStatus);
        FoodStatus = Mathf.Min(FoodStatus + randomValue, shipModel.ShipMaxFoodUpgradeList[shipMaxFoodCurrentLevel].Value);
        DemoController.Instance.shipController.shipStatusUI.ShowStatChange(0,0,Mathf.FloorToInt(FoodStatus) - prevFoodStatus, 0, 0, 0);
    }

    public void AddShipMoral(float randomValue)
    {
        int prevMoralStatus = Mathf.FloorToInt(MoralStatus);
        MoralStatus = Mathf.Min(MoralStatus + randomValue, shipModel.ShipMaxMoralUpgradeList[shipMaxMoralCurrentLevel].Value);
        DemoController.Instance.shipController.shipStatusUI.ShowStatChange(0, 0, 0, Mathf.FloorToInt(MoralStatus) - prevMoralStatus, 0, 0);
    }

    public void AddViewRange(float randomValue)
    {
        temporaryViewRangeAddition++;
    }

    public void AddTemporaryCanon(float randomValue)
    {
        temporaryCanonAddition++;
        DemoController.Instance.shipController.shipStatusUI.ShowStatChange(0, 0, 0, 0, Mathf.FloorToInt(randomValue), 0);
    }

    public void AddGold(float randomValue)
    {
        Gold = Gold + Mathf.FloorToInt(randomValue);
        DemoController.Instance.shipController.shipStatusUI.ShowStatChange(0, 0, 0, 0, 0, Mathf.FloorToInt(randomValue));
    }

    internal int GetUpgradeableSingleStatCurrent(EnumEventModifierRewardType shipStatModifier)
    {
        if (shipStatModifier.Equals(EnumEventModifierRewardType.Durability))
        {
            return shipDurabilityCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Crew))
        {
            return shipMaxCrewCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Food))
        {
            return shipMaxFoodCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Moral))
        {
            return shipMaxMoralCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Canons))
        {
            return shipMaxCanonsCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.ViewRange))
        {
            return shipViewRangeCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.DiscoverRange))
        {
            return shipDiscoverRangeCurrentLevel;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Speed))
        {
            return shipSpeedCurrentLevel; ;
        }
        return -1;
    }

    internal List<ShipSingleStatUpgadeItem> GetUpgradeableSingleStatList(EnumEventModifierRewardType shipStatModifier)
    {
        if (shipStatModifier.Equals(EnumEventModifierRewardType.Durability))
        {
            return this.shipModel.ShipDurabilityUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Crew))
        {
            return this.shipModel.ShipMaxCrewUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Food))
        {
            return this.shipModel.ShipMaxFoodUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Moral))
        {
            return this.shipModel.ShipMaxMoralUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Canons))
        {
            return this.shipModel.ShipMaxCanonsUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.ViewRange))
        {
            return this.shipModel.ShipViewRangeUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.DiscoverRange))
        {
            return this.shipModel.ShipDiscoverRangeUpgradeList;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Speed))
        {
            return this.shipModel.ShipSpeedUpgradeList;
        }
        return null;
    }

    internal void SetUpgradeableSingleStat(EnumEventModifierRewardType shipStatModifier, int value)
    {
        if (shipStatModifier.Equals(EnumEventModifierRewardType.Durability))
        {
            shipDurabilityCurrentLevel = value;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Crew))
        {
            shipMaxCrewCurrentLevel = value;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Food))
        {
            shipMaxFoodCurrentLevel = value;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Moral))
        {
            shipMaxMoralCurrentLevel = value;
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Canons))
        {
            shipMaxCanonsCurrentLevel= value;   
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.ViewRange))
        {
            shipViewRangeCurrentLevel= value;   
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.DiscoverRange))
        {
            shipDiscoverRangeCurrentLevel= value;   
        }
        else if (shipStatModifier.Equals(EnumEventModifierRewardType.Speed))
        {
            shipSpeedCurrentLevel= value;   
        }
    }

    internal int GetCurrentDiscoverRange()
    {
        if (shipModel == null)
        {
            return 0;
        }
        return shipModel.ShipDiscoverRangeUpgradeList[shipDiscoverRangeCurrentLevel].Value + temporaryViewRangeAddition;
    }

    internal int GetCurrentViewRange()
    {
        if (shipModel == null)
        {
            return 0;
        }
        return shipModel.ShipViewRangeUpgradeList[shipViewRangeCurrentLevel].Value + temporaryViewRangeAddition;
    }

    internal int GetCurrentMaxDurability()
    {
        if (shipModel == null)
        {
            return 0;
        }
        return shipModel.ShipDurabilityUpgradeList[shipDurabilityCurrentLevel].Value;
    }

    internal int GetCurrentMaxCrew()
    {
        if (shipModel == null)
        {
            return 0;
        }
        return shipModel.ShipMaxCrewUpgradeList[shipMaxCrewCurrentLevel].Value;
    }

    internal int GetCurrentMaxFood()
    {
        if (shipModel == null)
        {
            return 0;
        }
        return shipModel.ShipMaxFoodUpgradeList[shipMaxFoodCurrentLevel].Value;
    }

    internal int GetCurrentMaxMoral()
    {
        if (shipModel == null)
        {
            return 0;
        }
        return shipModel.ShipMaxMoralUpgradeList[shipMaxMoralCurrentLevel].Value;
    }

    internal int GetCurrentMaxCanons()
    {
        if (shipModel == null)
        {
            return 0;
        }
        return shipModel.ShipMaxCanonsUpgradeList[shipMaxCanonsCurrentLevel].Value;
    }

    internal int GetCurrentCanons()
    {
        if (shipModel == null)
        {
            return 0;
        }
        return shipModel.ShipMaxCanonsUpgradeList[shipMaxCanonsCurrentLevel].Value + temporaryCanonAddition;
    }
}
