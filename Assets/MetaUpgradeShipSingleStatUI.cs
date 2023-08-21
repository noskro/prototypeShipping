using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MetaUpgradeShipSingleStatUI : MonoBehaviour
{
    public string SingleStatName;
    public EnumShipStatModifierType shipStatModifier;

    public TextMeshProUGUI textSingleStatName;
    public TextMeshProUGUI textSingleStatCurrentLevel;
    public TextMeshProUGUI textSingleStatUpgradeButton;

    private int iCurrentLevel = -1;
    private int priceNextLevel = -1;

    // Start is called before the first frame update
    void Start()
    {
        MetaUpgradeUI.OnMetaUpgradeDialogStatusChanged += (state) =>
        {
            if (state)
            {
                ShowUpgradeShipStatData();
            }
        };
    }

    private void ShowUpgradeShipStatData()
    {
        textSingleStatName.text = SingleStatName;
        List<ShipSingleStatUpgadeItem> upgradeList = DemoController.Instance.shipStats.GetUpgradeableSingleStatList(shipStatModifier);
        iCurrentLevel = DemoController.Instance.shipStats.GetUpgradeableSingleStatCurrent(shipStatModifier);
        if (iCurrentLevel >= 0 && upgradeList != null && upgradeList.Count > 0)
        {
            textSingleStatCurrentLevel.text = iCurrentLevel + " / " + upgradeList.Count;
            if (iCurrentLevel + 1 < upgradeList.Count)
            {
                priceNextLevel = upgradeList[iCurrentLevel + 1].Price;
                textSingleStatUpgradeButton.text = "+ (" + priceNextLevel + " g)";
            }
            else
            {
                priceNextLevel = -1;
                textSingleStatUpgradeButton.text = "MAX";
            }
        }
        else
        {
            textSingleStatCurrentLevel.text = "";
            textSingleStatUpgradeButton.text = "";
        }
    }

    public void ClickUpgradeButton()
    {
        if (DemoController.Instance.shipStats != null)
        {
            int gold = DemoController.Instance.shipStats.Gold;

            if (priceNextLevel >= 0 && gold >= priceNextLevel)
            {
                DemoController.Instance.shipStats.Gold -= priceNextLevel;
                DemoController.Instance.shipStats.SetUpgradeableSingleStat(shipStatModifier, iCurrentLevel + 1);

                if (shipStatModifier.Equals(EnumShipStatModifierType.ViewRange))
                {
                    DemoController.Instance.shipStats.SetUpgradeableSingleStat(EnumShipStatModifierType.DiscoverRange, iCurrentLevel + 1);
                }
            }
        }

        ShowUpgradeShipStatData();
    }
}
