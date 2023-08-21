using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipStatsUI : MonoBehaviour
{
    public ShipStats shipStats;
    public TextMeshProUGUI textShowStats;
    public RunEndUIController runEndUIController;

    private void OnEnable()
    {
        ShipStats.OnShipUpdated += OnShipUpdated;
    }

    private void OnDisable()
    {
        ShipStats.OnShipUpdated -= OnShipUpdated;
    }

    private void OnShipUpdated(ShipStats stats)
    {
        bool shipLost = DemoController.Instance.GameState == EnumGameStates.ShipLost;

        if (!shipLost)
        {
            textShowStats.text = string.Format("Schiff: {0}/{1}\nCrew: {2}/{3}\nNahrung: {4}/{5}\nMoral {6}/{7}\nCanons: {8}\nGold: {9}",
                Mathf.Ceil(shipStats.ShipDurability), shipStats.GetCurrentMaxDurability(),
                Mathf.Ceil(shipStats.CrewCount), shipStats.GetCurrentMaxCrew(),
                Mathf.Ceil(shipStats.FoodStatus), shipStats.GetCurrentMaxFood(),
                Mathf.Ceil(shipStats.MoralStatus), shipStats.GetCurrentMaxMoral(),
                shipStats.GetCurrentMaxCanons(), 
                shipStats.Gold);
        }
    }

}
