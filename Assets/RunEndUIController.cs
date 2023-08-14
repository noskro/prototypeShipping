using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunEndUIController : MonoBehaviour
{
    public Transform ShipStatsPanel;

    private TextMeshProUGUI textGold;

    private TextMeshProUGUI textButtonUpgradeDocks;
    private DocksShipStatsUI dockShipCurrentShip;
    private DocksShipStatsUI dockShipNextShip;

    private ShipStats currentShipStats;

    private CanvasGroup canvasGroup;
    private float flowInValue;

    private void Start()
    {
        this.gameObject.SetActive(true);
        canvasGroup = this.GetComponent<CanvasGroup>();
        textGold = transform.Find("TotalGold").Find("TextGold").GetComponent<TextMeshProUGUI>();

        textButtonUpgradeDocks = this.transform.Find("ButtonUpgradeDocks").GetComponentInChildren<TextMeshProUGUI>();
        dockShipCurrentShip = transform.Find("DocksCurrentShip").GetComponent<DocksShipStatsUI>();
        dockShipCurrentShip.Init();
        dockShipNextShip = transform.Find("DocksNextShip").GetComponent<DocksShipStatsUI>();
        dockShipNextShip.Init();

        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (flowInValue >= 0)
        {
            flowInValue -= Time.deltaTime;

            if (flowInValue <= 1f)
            {
                this.canvasGroup.alpha = Mathf.Lerp(1, 0, flowInValue);
            }
        }

    }

    public void RunEnded(ShipStats stats)
    {
        currentShipStats = stats;

        ShipStatsPanel.gameObject.SetActive(false);

        textGold.text = stats.Gold.ToString();

        ShowUpgradeDockStatus();

        
        this.gameObject.SetActive(true);
        this.canvasGroup.alpha = 0;
        flowInValue = 1.5f; 
    }

    public void ClickUpgradeDocks()
    {
        ShipModelSO newShipModel = DemoController.Instance.GetNextShipModel();

        if (currentShipStats.Gold >= newShipModel.ShipPrice)
        {
            DemoController.Instance.currentShipModel = newShipModel;
            currentShipStats.shipModel = newShipModel;

            currentShipStats.Gold -= newShipModel.ShipPrice;
            textGold.text = currentShipStats.Gold.ToString();
        }

        ShowUpgradeDockStatus();
    }

    private void ShowUpgradeDockStatus()
    {
        dockShipCurrentShip.ShowShipModelStats(currentShipStats.shipModel);
        dockShipNextShip.ShowShipModelStats(DemoController.Instance.GetNextShipModel());

        ShipModelSO nextShipModel = DemoController.Instance.GetNextShipModel(); // shipProgressionList.IndexOf(currentShipStats.shipModel);

        if (nextShipModel != null)
        {
            textButtonUpgradeDocks.text = "Upgrade Docks (" + nextShipModel.ShipPrice + " gold)";            
        }
        else
        {
            textButtonUpgradeDocks.text = "Docks at maximum";
        }
    }

    public void StartNewRun()
    {
        this.gameObject.SetActive(false);
        ShipStatsPanel.gameObject.SetActive(true);
        DemoController.Instance.PlaceNewShip();
    }
}
