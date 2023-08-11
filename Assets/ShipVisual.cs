using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipVisual : MonoBehaviour
{
    private SpriteRenderer shipSpriteRenderer;

    public Sprite shipIdle;
    public Sprite shipMoving;
    public Sprite shipLost;

    // Start is called before the first frame update
    void Start()
    {
        shipSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        shipSpriteRenderer.sprite = shipIdle;
    }

    private void Update()
    {
        if (GetComponent<ShipStats>().shipLost)
        {
            shipSpriteRenderer.sprite = shipLost;
        }
    }

    public void ShowShipidle()
    {
        shipSpriteRenderer.sprite = shipIdle;
    }

    public void ShowShipMoving(bool moving)
    {
        if (moving)
        {
            shipSpriteRenderer.sprite = shipMoving;
        }
        else
        {
            shipSpriteRenderer.sprite = shipIdle;
        }
    }
}
