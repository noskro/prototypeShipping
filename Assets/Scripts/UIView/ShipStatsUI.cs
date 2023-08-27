using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipStatsUI : MonoBehaviour
{
    public ShipStats shipStats;
    public TextMeshProUGUI textShowStats;
    public TextMeshProUGUI textShowStatChangePositive;
    public TextMeshProUGUI textShowStatChangeNegative;
    public TextMeshProUGUI textShowStatChangePossible;
    public Button buttonEndRund;
    public RunEndUIController runEndUIController;

    private float statChangeTimeout;

    private void OnEnable()
    {
        ShipController.OnShipUpdated += OnShipUpdated;
    }

    private void OnDisable()
    {
        ShipController.OnShipUpdated -= OnShipUpdated;
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

            c = textShowStatChangePossible.color;
            c.a = 0;
            textShowStatChangePossible.color = c;
        }
        else
        {
            Color c = textShowStatChangePossible.color;
            c.a = 1;
            textShowStatChangePossible.color = c;
        }
    }

    private void OnShipUpdated(ShipStats stats)
    {
        bool shipLost = DemoController.Instance.GameState == EnumGameStates.ShipLost;

        if (!shipLost)
        {
            textShowStats.text = string.Format("Durability: {0}/{1}\nCrew: {2}/{3}\nCannons: {8}\nGold: {9}", /*Moral {6}/{7}\n Food: { 4}/{ 5}\n */
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

    public void ShowPossibleStatChange(int? durabilityChange, int? crewChange, int? foodChange, int? moralChange, int? canonChange, int? goldChange)
    {
        if (durabilityChange == null && crewChange == null && foodChange == null && moralChange == null && canonChange == null && goldChange == null)
        {
            textShowStatChangePossible.text = "";
        }
        else
        {
            string dur = (durabilityChange != null) ? (durabilityChange > 0) ? "+" + durabilityChange : "" + durabilityChange : "~";
            string crew = (crewChange != null) ? (crewChange > 0) ? "+" + crewChange : "" + crewChange : "~";
            string food = (foodChange != null) ? (foodChange > 0) ? "+" + foodChange : "" + foodChange : "~";
            string mor = (moralChange != null) ? (moralChange > 0) ? "+" + moralChange : "" + moralChange : "~";
            string can = (canonChange != null) ? (canonChange > 0) ? "+" + canonChange : "" + canonChange : "~";
            string gold = (goldChange != null) ? (goldChange > 0) ? "+" + goldChange : "" + goldChange : "~";
            ShowPossibleStatChangeString(dur, crew, food, mor, can, gold);
        }
    }

    public void ShowPossibleStatChangeString(string durabilityChange, string crewChange, string foodChange, string moralChange, string canonChange, string goldChange)
    {
        if (durabilityChange == null && crewChange == null && foodChange == null && moralChange == null && canonChange == null && goldChange == null)
        {
            textShowStatChangePossible.text = "";
        }
        else
        {
            textShowStatChangePossible.text = string.Format("{0}\n{1}\n{4}\n{5}\n",  /*\n{ 2}*/
                    (durabilityChange != null) ? durabilityChange : "~",
                    (crewChange != null) ? crewChange : "~",
                    (foodChange != null) ? foodChange : "~",
                    (moralChange != null) ? moralChange : "~",
                    (canonChange != null) ? canonChange : "~",
                    (goldChange != null) ? goldChange : "~");
            textShowStatChangePositive.text = "";
            textShowStatChangeNegative.text = "";
        }
    }

    public void ShowStatChange(int durabilityChange, int crewChange, int foodChange, int moralChange, int canonChange, int goldChange)
    {
        textShowStatChangePositive.text = string.Format("{0}\n{1}\n{4}\n{5}\n", /*\n{ 2}*/
                (durabilityChange > 0) ? "+" + durabilityChange : " ",
                (crewChange > 0) ? "+" + crewChange : " ",
                (foodChange > 0) ? "+" + foodChange : " ",
                (moralChange > 0) ? "+" + moralChange : " ",
                (canonChange > 0) ? "+" + canonChange : " ",
                (goldChange > 0) ? "+" + goldChange : ".");

        textShowStatChangeNegative.text = string.Format("{0}\n {1}\n {4}\n {5}\n",  /*{ 2}\n*/
                (durabilityChange < 0) ? "" + durabilityChange : " ",
                (crewChange < 0) ? "" + crewChange : " ",
                (foodChange < 0) ? "" + foodChange : " ",
                (moralChange < 0) ? "" + moralChange : " ",
                (canonChange < 0) ? "" + canonChange : " ",
                (goldChange < 0) ? "" + goldChange : ".");

        statChangeTimeout = 3f;
    }

    public void ClickEndRun()
    {
        DemoController.Instance.metaUpgrade.Show();
    }
}
