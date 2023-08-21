using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeController : MonoBehaviour
{
    public SpriteRenderer trade1; 
    public SpriteRenderer trade2; 
    public SpriteRenderer trade3;

    public Sprite spriteTradeGold;
    public Sprite spriteTradeFood;
    public Sprite spriteTradeRepair;
    public Sprite spriteTradeRum;
    public Sprite spriteTradeCrew;

    public ShipStats shipStats;

    public bool IsTrading { get; internal set; }
    private PersistentCityData villageGameMapData;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowTrade(Vector3 pos, PersistentCityData dataVillage)
    {
        this.villageGameMapData = dataVillage;
        IsTrading = true;
        this.transform.position = pos;

        trade1.sprite = spriteTradeGold;

        int selection2 = Random.Range(0,2);
        int selection3 = selection2;

        while (selection3 == selection2)
        {
            selection3 = Random.Range(0, 2);
        }

        if (selection2 == 0)
        {
            trade2.sprite = spriteTradeFood;
        }
        else if (selection2 == 1)
        {
            trade2.sprite = spriteTradeRepair;
        }
        else if (selection2 == 2)
        {
            trade2.sprite = spriteTradeRum;
        }
        else if (selection2 == 3)
        {
            trade2.sprite = spriteTradeCrew;
        }

        if (selection3 == 0)
        {
            trade3.sprite = spriteTradeFood;
        }
        else if (selection3 == 1)
        {
            trade3.sprite = spriteTradeRepair;
        }
        else if (selection3 == 2)
        {
            trade3.sprite = spriteTradeRum;
        }
        else if (selection3 == 3)
        {
            trade3.sprite = spriteTradeCrew;
        }

        this.gameObject.SetActive(true);
    }

    public void ClickTrade(int id)
    {
        if (id == 0)
        {
            IsTrading = false;
            this.gameObject.SetActive(false);
            return;
        }

        SpriteRenderer selectedSR = null;
        if (id == 1)
        {
            selectedSR = trade1;
        }
        else if (id == 2)
        {
            selectedSR = trade2;
        }
        else if (id == 3)
        {
            selectedSR = trade3;
        }

        if (selectedSR.sprite.Equals(spriteTradeGold))
        {
            shipStats.Gold += Random.Range(8,14);
        }
        else if (selectedSR.sprite.Equals(spriteTradeRepair))
        {
            shipStats.ShipDurability = Mathf.Min(shipStats.ShipDurability + 1, shipStats.GetCurrentMaxDurability());
        }
        else if (selectedSR.sprite.Equals(spriteTradeCrew))
        {
            shipStats.CrewCount = Mathf.Min(shipStats.CrewCount + 1, shipStats.GetCurrentMaxMoral());
        }
        else if (selectedSR.sprite.Equals(spriteTradeFood))
        {
            shipStats.FoodStatus = Mathf.Min(shipStats.FoodStatus + 2, shipStats.GetCurrentMaxFood());
        }
        else if (selectedSR.sprite.Equals(spriteTradeRum))
        {
            shipStats.MoralStatus = Mathf.Min(shipStats.MoralStatus + Random.Range(1,2), shipStats.GetCurrentMaxMoral());
        }

        shipStats.TriggerShipUpdated();

        villageGameMapData.SetTradedThisRun(true);
        IsTrading = false;
        this.gameObject.SetActive(false);
    }
}
