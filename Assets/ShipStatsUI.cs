using TMPro;
using UnityEngine;

public class ShipStatsUI : MonoBehaviour
{

    public ShipStats shipStats;
    public TextMeshProUGUI textShowStats;

    // Update is called once per frame
    void Update()
    {
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
