using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CartographyLevelUI : MonoBehaviour
{
    private TextMeshProUGUI textCartographyLevelName;
    private TextMeshProUGUI textCartographyLevelRadius;

    internal void Init()
    {
        textCartographyLevelName = this.transform.Find("TextCartographyLevelName").GetComponent<TextMeshProUGUI>();
        textCartographyLevelRadius = this.transform.Find("TextCartographyLevelRadius").GetComponent<TextMeshProUGUI>();
    }

    internal void ShowCartographyLevel(CartographyLevelSO cartographyLevel)
    {
        textCartographyLevelName.text = cartographyLevel.CartographyLevelName;
        textCartographyLevelRadius.text = "Radius: " + cartographyLevel.CartographyRadius;
    }
}
