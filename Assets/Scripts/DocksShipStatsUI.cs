using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DocksShipStatsUI : MonoBehaviour
{

    private TextMeshProUGUI textShipName;
    private TextMeshProUGUI textShipDurability;
    private TextMeshProUGUI textShipMaxCrew;
    private TextMeshProUGUI textShipMaxFood;
    private TextMeshProUGUI textShipSpeed;
    private TextMeshProUGUI textShipViewRange;
    private TextMeshProUGUI textShipDiscoverRange;

    public  void Init()
    {
        textShipName = this.transform.Find("TextShipName").GetComponent<TextMeshProUGUI>();
        textShipDurability = this.transform.Find("TextShipDurability").GetComponent<TextMeshProUGUI>();
        textShipMaxCrew = this.transform.Find("TextShipMaxCrew").GetComponent<TextMeshProUGUI>();
        textShipMaxFood = this.transform.Find("TextShipMaxFood").GetComponent<TextMeshProUGUI>();
        textShipSpeed = this.transform.Find("TextShipSpeed").GetComponent<TextMeshProUGUI>();
        textShipViewRange = this.transform.Find("TextShipViewRange").GetComponent<TextMeshProUGUI>();
        textShipDiscoverRange = this.transform.Find("TextShipDiscoverRange").GetComponent<TextMeshProUGUI>();
    }

    public void ShowShipModelStats(ShipModelSO model)
    {
        textShipName.text = model.ShipModelName;
        textShipDurability.text = "Rumpf: " + model.ShipDurabilityMax;
        textShipMaxCrew.text = "Crew: " + model.CrewCountMax;
        textShipMaxFood.text = "Nahrung: " + model.FoodStatusMax;
        textShipSpeed.text = "Geschw.:" + model.ShipSpeed;
        textShipViewRange.text = "Sicht:" + model.ViewRange;
        textShipDiscoverRange.text = "Erkunden:" + model.DiscoverRange;
    }
}
