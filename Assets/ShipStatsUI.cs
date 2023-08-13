using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipStatsUI : MonoBehaviour
{

    public ShipStats shipStats;
    public TextMeshProUGUI textShowStats;
    public Button buttonPlaceNewShip;

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
        bool shipLost = DemoController.Instance.GameState == DemoController.GameStates.ShipLost;

        buttonPlaceNewShip.gameObject.SetActive(shipLost);

        if (shipLost)
        {
            textShowStats.text = "";
        }
        else
        {
            textShowStats.text = string.Format("Schiff: {0}/{1}\nCrew: {2}/{3}\nNahrung: {4}/{5}\nMoral {6}/{7}\nGold: {8}",
                Mathf.Ceil(shipStats.ShipStatus), shipStats.shipModel.ShipStatusMax,
                Mathf.Ceil(shipStats.CrewCount), shipStats.shipModel.CrewCountMax,
                Mathf.Ceil(shipStats.FoodStatus), shipStats.shipModel.FoodStatusMax,
                Mathf.Ceil(shipStats.MoralStatus), shipStats.shipModel.MoralStatusMax, shipStats.Gold);
        }
    }

}
