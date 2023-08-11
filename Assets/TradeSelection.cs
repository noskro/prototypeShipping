using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeSelection : MonoBehaviour
{
    public TradeController tradeController;
    public int id;

    private void OnMouseUp()
    {
        tradeController.ClickTrade(id);
    }
}
