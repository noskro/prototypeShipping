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
            shipStats.Gold += Random.Range(1,1);
        }
        else if (selectedSR.sprite.Equals(spriteTradeGoldUpgraded))
        {
            shipStats.Gold += Random.Range(2,3);
        }
        else if (selectedSR.sprite.Equals(spriteTradeRepair))
        {
            shipStats.ShipDurability = Mathf.Min(shipStats.ShipDurability + 1, shipStats.GetCurrentMaxDurability());
        }
        else if (selectedSR.sprite.Equals(spriteTradeCrew))
        {
            shipStats.CrewCount = Mathf.Min(shipStats.CrewCount + 1, shipStats.GetCurrentMaxMoral());
        }
        else if (selectedSR.sprite.Equals(spriteTradeQuest))
        {
            int eventIndex = Random.Range(-1, villageGameMapData.TavernStoryTextEvents.Count - 1);

            if (eventIndex >= 0)
            {
                DemoController.Instance.storyTextManager.ActiveStoryTextEventList.Add(villageGameMapData.TavernStoryTextEvents[eventIndex]);
                villageGameMapData.TavernStoryTextEvents.RemoveAt(eventIndex);
            }
            else
            {
                // todo get default tavern quest
                eventIndex = Random.Range(0, DefaultTavernQuests.Count - 1);

                if (eventIndex >= 0)
                {
                    DemoController.Instance.storyTextManager.ActiveStoryTextEventList.Add(DefaultTavernQuests[eventIndex]);
                }
            }
        }
        else if (selectedSR.sprite.Equals(spriteTradeFood))
        {
            shipStats.FoodStatus = Mathf.Min(shipStats.FoodStatus + 2, shipStats.GetCurrentMaxFood());
        }
        else if (selectedSR.sprite.Equals(spriteTradeRum))
        {
            shipStats.MoralStatus = Mathf.Min(shipStats.MoralStatus + Random.Range(1,2), shipStats.GetCurrentMaxMoral());
        }
        else if (selectedSR.sprite == null)
        {
            return; // do nothing; 
        }

        DemoController.Instance.shipController.ResetShipPosition();
        shipStats.TriggerShipUpdated();

        villageGameMapData.SetTradedThisRun(true);
        IsTrading = false;
        this.gameObject.SetActive(false);
        DemoController.Instance.storyTextManager.CheckForNewEvents();
    }
}
