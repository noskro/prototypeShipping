using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipStatsUI : MonoBehaviour
{
    public ShipStats shipStats;
    public TextMeshProUGUI textShowStats;
    public TextMeshProUGUI textShowStatChangePositive;
    public TextMeshProUGUI textShowStatChangeNegative;
    public Button buttonEndRund;
    public RunEndUIController runEndUIController;

    private float statChangeTimeout;

    private void OnEnable()
    {
        ShipStats.OnShipUpdated += OnShipUpdated;
    }

    private void OnDisable()
    {
        ShipStats.OnShipUpdated -= OnShipUpdated;
    }

    private void Start()
    {
        textShowStatChangePositive.text = "";
        textShowStatChangeNegative.text = "";
    }

    private void Update()
    {
        if (statChangeTimeout > 0)
        {
            statChangeTimeout -= Time.deltaTime;

            Color c = textShowStatChangePositive.color;
            c.a = Mathf.Lerp(0, 1, statChangeTimeout);
            textShowStatChangePositive.color = c;

            c = textShowStatChangeNegative.color;
            c.a = Mathf.Lerp(0, 1, statChangeTimeout);
            textShowStatChangeNegative.color = c;
        }
    }

    private void OnShipUpdated(ShipStats stats)
    {
        bool shipLost = DemoController.Instance.GameState == EnumGameStates.ShipLost;

        if (!shipLost)
        {
            textShowStats.text = string.Format("Schiff: {0}/{1}\nCrew: {2}/{3}\nNahrung: {4}/{5}\nCanons: {8}\nGold: {9}", /*Moral {6}/{7}\n*/
                Mathf.Ceil(shipStats.ShipDurability), shipStats.GetCurrentMaxDurability(),
                Mathf.Ceil(shipStats.CrewCount), shipStats.GetCurrentMaxCrew(),
                Mathf.Ceil(shipStats.FoodStatus), shipStats.GetCurrentMaxFood(),
                Mathf.Ceil(shipStats.MoralStatus), shipStats.GetCurrentMaxMoral(),
                shipStats.GetCurrentMaxCanons(),
                shipStats.Gold);

            buttonEndRund.gameObject.SetActive(false);
        }
        else
        {
            textShowStats.text = "";
            buttonEndRund.gameObject.SetActive(true);
        }
    }

    public void ShowStatChange(int durabilityChange, int crewChange, int foodChange, int moralChange, int canonChange, int goldChange)
    {
        textShowStatChangePositive.text = string.Format("{0}\n{1}\n{2}\n{4}\n{5}\n",
                (durabilityChange > 0) ? durabilityChange : " ",
                (crewChange > 0) ? "+" + crewChange : " ",
                (foodChange > 0) ? "+" + foodChange : " ",
                (moralChange > 0) ? "+" + moralChange : " ",
                (canonChange > 0) ? "+" + canonChange : " ",
                (goldChange > 0) ? "+" + goldChange : " ");

        textShowStatChangeNegative.text = string.Format("{0}\n {1}\n {2}\n {4}\n {5}\n",
                (durabilityChange < 0) ? "" + durabilityChange : ".",
                (crewChange < 0) ? "" + crewChange : ".",
                (foodChange < 0) ? "" + foodChange : ".",
                (moralChange < 0) ? "" + moralChange : ".",
                (canonChange < 0) ? "" + canonChange : ".",
                (goldChange < 0) ? "" + goldChange : ".");

        statChangeTimeout = 3f;
    }

    public void ClickEndRun()
    {
        DemoController.Instance.metaUpgrade.Show();
    }
}
