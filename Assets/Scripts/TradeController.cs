using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeController : MonoBehaviour
{
    public SpriteRenderer trade1; 
    public SpriteRenderer trade2; 
    public SpriteRenderer trade3;

    public Sprite spriteTradeGold;
    public Sprite spriteTradeGoldUpgraded;
    public Sprite spriteTradeFood;
    public Sprite spriteTradeRepair;
    public Sprite spriteTradeRum;
    public Sprite spriteTradeCrew;
    public Sprite spriteTradeQuest;

    public ShipStats shipStats;

    public bool IsTrading { get; internal set; }
    private PersistentCityData villageGameMapData;

    public List<StoryTextEventSO> DefaultTavernQuests;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowTrade(Vector3 pos, PersistentCityData cityData)
    {
        this.villageGameMapData = cityData;
        IsTrading = true;
        this.transform.position = pos;

        if (cityData.LockedTradesForThisRound == null || cityData.LockedTradesForThisRound.Count == 0)
        {
            if (!cityData.IndustryBuild)
            {
                trade1.sprite = spriteTradeGold;
            }
            else
            {
                trade1.sprite = spriteTradeGoldUpgraded;
            }

            int selection2 = SelectRandomTrade(cityData, trade2, -1);
            int selection3 = SelectRandomTrade(cityData, trade3, selection2);

            cityData.LockedTradesForThisRound = new List<Sprite>();
            cityData.LockedTradesForThisRound.Add(trade1.sprite); 
            cityData.LockedTradesForThisRound.Add(trade2.sprite); 
            cityData.LockedTradesForThisRound.Add(trade3.sprite); 
        }
        else
        {
            trade1.sprite = (cityData.LockedTradesForThisRound.Count >= 0)? cityData.LockedTradesForThisRound[0]: null;
            trade2.sprite = (cityData.LockedTradesForThisRound.Count >= 1) ? cityData.LockedTradesForThisRound[1] : null;
            trade3.sprite = (cityData.LockedTradesForThisRound.Count >= 2) ? cityData.LockedTradesForThisRound[2] : null;
        }

        this.gameObject.SetActive(true);
    }

    private int SelectRandomTrade(PersistentCityData cityData, SpriteRenderer spriteRendererTrade, int previousSelection)
    {
        /* Selection: 
         * 1 = dock
         * 2 = tavern
         * 3 = tavern quest
         * 4 = trader food
         * 5 = trader rum
         */

        for (int i = 0; i <= 10; i++)
        {
            int randomSelection = Random.Range(1, 5);

            if (randomSelection == previousSelection)
            {
                continue;
            }

            if (randomSelection == 1 && cityData.DocksBuild)
            {
                spriteRendererTrade.sprite = spriteTradeRepair;
                return randomSelection;
            }
            if (randomSelection == 2 && cityData.TavernBuild)
            {
                spriteRendererTrade.sprite = spriteTradeCrew;
                return randomSelection;
            }
            if (randomSelection == 3 && cityData.TavernBuild)
            {
                spriteRendererTrade.sprite = spriteTradeQuest; 
                return randomSelection;
            }
            if (randomSelection == 4 && cityData.TraderBuild)
            {
                spriteRendererTrade.sprite = spriteTradeFood;
                return randomSelection;
            }
            if (randomSelection == 5 && cityData.TraderBuild)
            {
                spriteRendererTrade.sprite = spriteTradeRum;
                return randomSelection;
            }
        }

        spriteRendererTrade.sprite = null;
        return -1;
    }

    public void MouseOverTrade(int id)
    {
        string tradeDurability = null;
        string tradeCrew = null;
        string tradeFood = null;
        string tradeMoral = null;
        string tradeCanons = null;
        string tradeGold = null;

        if (id == 0)
        {
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

        if (selectedSR.sprite != null)
        {
            if (selectedSR.sprite.Equals(spriteTradeGold))
            {
                tradeGold = "+1";
            }
            else if (selectedSR.sprite.Equals(spriteTradeGoldUpgraded))
            {
                tradeGold = "+(2-3)";
            }
            else if (selectedSR.sprite.Equals(spriteTradeRepair))
            {
                tradeDurability = "+" + (shipStats.GetCurrentMaxDurability() - shipStats.ShipDurability);
            }
            else if (selectedSR.sprite.Equals(spriteTradeCrew))
            {
                tradeCrew = "+" + (Mathf.Min(shipStats.CrewCount + 1, shipStats.GetCurrentMaxMoral()));
            }
            else if (selectedSR.sprite.Equals(spriteTradeFood))
            {
                tradeFood = "+" + (Mathf.Min(shipStats.FoodStatus + 2, shipStats.GetCurrentMaxFood()));
            }
            else if (selectedSR.sprite.Equals(spriteTradeRum))
            {
                tradeMoral = "+" + (Mathf.Min(shipStats.MoralStatus + Random.Range(1, 2), shipStats.GetCurrentMaxMoral()));
            }
        }
        DemoController.Instance.shipController.shipStatusUI.ShowPossibleStatChangeString(tradeDurability, tradeCrew, tradeFood, tradeMoral, tradeCanons, tradeGold);
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
            shipStats.AddGold(Random.Range(1,1));
        }
        else if (selectedSR.sprite.Equals(spriteTradeGoldUpgraded))
        {
            shipStats.AddGold(Random.Range(2,3));
        }
        else if (selectedSR.sprite.Equals(spriteTradeRepair))
        {
            shipStats.AddShipDurability((shipStats.GetCurrentMaxDurability() - shipStats.ShipDurability)); // Mathf.Min(shipStats.ShipDurability + 1, shipStats.GetCurrentMaxDurability()));
        }
        else if (selectedSR.sprite.Equals(spriteTradeCrew))
        {
            shipStats.AddShipCrew(Mathf.Min(shipStats.CrewCount + 1, shipStats.GetCurrentMaxMoral()));
        }
        else if (selectedSR.sprite.Equals(spriteTradeQuest))
        {
            if (villageGameMapData.TavernStoryTextEvents.Count > 0)
            { 
                int eventIndex = Random.Range(0, villageGameMapData.TavernStoryTextEvents.Count);

                DemoController.Instance.storyTextManager.ActiveStoryTextEventList.Add(villageGameMapData.TavernStoryTextEvents[eventIndex]);
                villageGameMapData.TavernStoryTextEvents.RemoveAt(eventIndex);
            }
            else
            {
                // todo get default tavern quest
                int eventIndex = Random.Range(0, DefaultTavernQuests.Count);
                DemoController.Instance.storyTextManager.ActiveStoryTextEventList.Add(DefaultTavernQuests[eventIndex]);
            }
        }
        else if (selectedSR.sprite.Equals(spriteTradeFood))
        {
            shipStats.AddShipFood(Mathf.Min(shipStats.FoodStatus + 2, shipStats.GetCurrentMaxFood()));
        }
        else if (selectedSR.sprite.Equals(spriteTradeRum))
        {
            shipStats.AddShipMoral(Mathf.Min(shipStats.MoralStatus + Random.Range(1,2), shipStats.GetCurrentMaxMoral()));
        }
        else if (selectedSR.sprite == null)
        {
            return; // do nothing; 
        }

        DemoController.Instance.shipController.ResetShipPosition();

        villageGameMapData.SetTradedThisRun(true);
        IsTrading = false;
        this.gameObject.SetActive(false);

        DemoController.Instance.shipController.EndTurn();
    }
}
