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

    private void OnShipUpdated()
    {
        buttonPlaceNewShip.gameObject.SetActive(shipStats.shipLost);

        if (!shipStats.shipLost)
        {
            textShowStats.text = string.Format("Schiff: {0}/{1}\nCrew: {2}/{3}\nNahrung: {4}/{5}\nMoral {6}/{7}\nGold: {8}",
                Mathf.Ceil(shipStats.ShipStatus), shipStats.currentShip.ShipStatusMax,
                Mathf.Ceil(shipStats.CrewCount), shipStats.currentShip.CrewCountMax,
                Mathf.Ceil(shipStats.FoodStatus), shipStats.currentShip.FoodStatusMax,
                Mathf.Ceil(shipStats.MoralStatus), shipStats.currentShip.MoralStatusMax, shipStats.Gold);
        }
        else
        {
            textShowStats.text = "";
        }
    }

}
