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
    public DocksShipStatsUI dockShipCurrentShip;
    public DocksShipStatsUI dockShipNextShip;

    private TextMeshProUGUI textButtonUpgradeCartograph;
    public CartographyLevelUI cartographyCurrentLevel;
    public CartographyLevelUI cartographyNextLevel;


    private ShipStats currentShipStats;

    private CanvasGroup canvasGroup;
    private float flowInValue;

    private void Start()
    {
        this.gameObject.SetActive(true);
        canvasGroup = this.GetComponent<CanvasGroup>();
        textGold = transform.Find("TotalGold").Find("TextGold").GetComponent<TextMeshProUGUI>();

        textButtonUpgradeDocks = this.transform.Find("PanelDock").Find("ButtonUpgradeDocks").GetComponentInChildren<TextMeshProUGUI>();
        dockShipCurrentShip.Init();
        dockShipNextShip.Init();

        textButtonUpgradeCartograph = this.transform.Find("PanelCartograph").Find("ButtonUpgradeCartograph").GetComponentInChildren<TextMeshProUGUI>();
        cartographyCurrentLevel.Init();
        cartographyNextLevel.Init();

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

        ShowData();
        
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

        ShowData();
    }

    public void ClickUpgradeCartography()
    {
        CartographyLevelSO nextCartographyLevel = DemoController.Instance.GetNextCartograhpyLevel();

        if (currentShipStats.Gold >= nextCartographyLevel.UpgradePrice)
        {
            DemoController.Instance.UpgradeCartographyLevel();

            currentShipStats.Gold -= nextCartographyLevel.UpgradePrice;
            textGold.text = currentShipStats.Gold.ToString();
        }

        ShowData();
    }

    private void ShowData()
    {
        textGold.text = currentShipStats.Gold.ToString();

        ShowUpgradeDockStatus();
        ShowUpgradeCartographyStatus();
    }

    private void ShowUpgradeDockStatus()
    {
        dockShipCurrentShip.ShowShipModelStats(currentShipStats.shipModel);
        dockShipNextShip.ShowShipModelStats(DemoController.Instance.GetNextShipModel());

        ShipModelSO nextShipModel = DemoController.Instance.GetNextShipModel(); // shipProgressionList.IndexOf(currentShipStats.shipModel);

        if (nextShipModel != null)
        {
            textButtonUpgradeDocks.text = "Werft verbessern (" + nextShipModel.ShipPrice + " Gold)";            
        }
        else
        {
            textButtonUpgradeDocks.text = "Werft auf max. Stufe";
        }
    }

    private void ShowUpgradeCartographyStatus()
    {
        cartographyCurrentLevel.ShowCartographyLevel(DemoController.Instance.GetCurrentCartograhpyLevel());
        cartographyNextLevel.ShowCartographyLevel(DemoController.Instance.GetNextCartograhpyLevel());

        CartographyLevelSO nextCartographyLevel = DemoController.Instance.GetNextCartograhpyLevel();

        if (nextCartographyLevel != null)
        {
            textButtonUpgradeCartograph.text = "Kartograph aufwerten (" + nextCartographyLevel.UpgradePrice + " Gold)";            
        }
        else
        {
            textButtonUpgradeCartograph.text = "Kartograph auf max. Stufe";
        }
    }

    public void StartNewRun()
    {
        this.gameObject.SetActive(false);
        ShipStatsPanel.gameObject.SetActive(true);
        DemoController.Instance.PlaceNewShip();
    }
}
