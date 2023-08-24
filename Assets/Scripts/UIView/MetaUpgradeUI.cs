using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DemoController;

public class MetaUpgradeUI : MonoBehaviour
{
    public TextMeshProUGUI textGold;

    public delegate void MetaUpgradeDialogStatusChanges(bool open);
    public static event MetaUpgradeDialogStatusChanges OnMetaUpgradeDialogStatusChanged;

    private bool active = false;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    public void Show()
    {
        active = true;
        this.gameObject.SetActive(true);
        StaticTileDataContainer.Instance.TilemapFOW.gameObject.SetActive(false);
        OnMetaUpgradeDialogStatusChanged?.Invoke(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (active && DemoController.Instance.shipController.shipStats != null)
        {
            textGold.text = "GOLD: " + DemoController.Instance.shipController.shipStats.Gold;
        }
    }

    public void ClickButtonContinue()
    {
        active = false;
        StaticTileDataContainer.Instance.TilemapFOW.gameObject.SetActive(true);
        OnMetaUpgradeDialogStatusChanged?.Invoke(false);
        this.gameObject.SetActive(false);
        DemoController.Instance.GenerateNewRun();
    }
}
