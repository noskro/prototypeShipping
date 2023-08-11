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

    public ShipModelSO currentShip;

    [Header("Settings")]
    public float shipDamagePerDayfromSailing;
    public float foodPerCrewPerDay;
    public float moralLossOnSea;
    public float moralLossAtCoast;

    [Header("References")]
    public Button buttonPlaceNewShip;
    public TextMeshProUGUI textShowStats;

    private ShipVisual shipVisual;

    [HideInInspector]
    public bool shipLost = true;

    public GameMapHandler.Direction? direction;


    private void Awake()
    {
        shipVisual = GetComponent<ShipVisual>();
    }

    public void SetShip(ShipModelSO newShipModel)
    {
        this.currentShip = newShipModel;
        shipVisual.ShowShipidle();

        ShipStatus = currentShip.ShipStatusMax;
        CrewCount = (int)Mathf.Ceil(currentShip.CrewCountMax / 2);
        FoodStatus = currentShip.FoodStatusMax;
        MoralStatus = currentShip.MoralStatusMax;

        shipLost = false;
        buttonPlaceNewShip.gameObject.SetActive(false);

    }

    public void NextTurn(CustomTile newTile)
    {
        float partOfDay = 1f / currentShip.ShipSpeed;

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

            buttonPlaceNewShip.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shipLost == false)
        {
            textShowStats.text = string.Format("Schiff: {0}/{1}\nCrew: {2}/{3}\nNahrung: {4}/{5}\nMoral {6}/{7}\nGold: {8}",
                Mathf.Ceil(ShipStatus), currentShip.ShipStatusMax,
                Mathf.Ceil(CrewCount), currentShip.CrewCountMax,
                Mathf.Ceil(FoodStatus), currentShip.FoodStatusMax,
                Mathf.Ceil(MoralStatus), currentShip.MoralStatusMax,
                Gold);
        }
        else
        {
            textShowStats.text = "";
        }
    }
}
