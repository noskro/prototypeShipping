using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaUpgradeShipSingleStatUI : MonoBehaviour
{
    public EnumStoryProgressionTags showOnlyOnTag;

    public string SingleStatName;
    public EnumEventModifierRewardType shipStatModifier;

    public Image imageBackground;
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
        if (!showOnlyOnTag.Equals(EnumStoryProgressionTags.None) && !DemoController.Instance.StoryProgressionTags.Contains(showOnlyOnTag))
        {
            textSingleStatUpgradeButton.GetComponentInParent<Button>().gameObject.SetActive(false);
            imageBackground.enabled = false;
            textSingleStatName.gameObject.SetActive(false);
            textSingleStatCurrentLevel.gameObject.SetActive(false);
        }
        else
        {
            imageBackground.gameObject.SetActive(true);
            imageBackground.enabled = true;
            textSingleStatCurrentLevel.gameObject.SetActive(true);
            textSingleStatUpgradeButton.GetComponentInParent<Button>().gameObject.SetActive(true);

            textSingleStatName.text = SingleStatName;
            List<ShipSingleStatUpgadeItem> upgradeList = DemoController.Instance.shipController.shipStats.GetUpgradeableSingleStatList(shipStatModifier);
            iCurrentLevel = DemoController.Instance.shipController.shipStats.GetUpgradeableSingleStatCurrent(shipStatModifier);
            if (iCurrentLevel >= 0 && upgradeList != null && upgradeList.Count > 0)
            {
                textSingleStatCurrentLevel.text = iCurrentLevel + " / " + (upgradeList.Count - 1);
                if (iCurrentLevel < upgradeList.Count - 1)
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
    }

    public void ClickUpgradeButton()
    {
        if (DemoController.Instance.shipController.shipStats != null)
        {
            int gold = DemoController.Instance.shipController.shipStats.Gold;

            if (priceNextLevel >= 0 && gold >= priceNextLevel)
            {
                DemoController.Instance.shipController.shipStats.Gold -= priceNextLevel;
                DemoController.Instance.shipController.shipStats.SetUpgradeableSingleStat(shipStatModifier, iCurrentLevel + 1);

                if (shipStatModifier.Equals(EnumEventModifierRewardType.ViewRange))
                {
                    DemoController.Instance.shipController.shipStats.SetUpgradeableSingleStat(EnumEventModifierRewardType.DiscoverRange, iCurrentLevel + 1);
                }
            }
        }

        ShowUpgradeShipStatData();
    }
}
