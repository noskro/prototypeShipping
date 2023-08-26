using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowTooltipUI : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
{
    public TextMeshProUGUI text;
    public string tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.text.text = tooltip;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.text.text = "";
    }

    internal void SetText(TextMeshProUGUI textUpgradeDescription)
    {
        this.text = textUpgradeDescription;
    }
}
