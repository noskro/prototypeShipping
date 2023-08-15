
using System;
using UnityEngine;

public class GameMapData
{
    public EnumFogOfWar fow = EnumFogOfWar.Undiscovered;
    public bool hasVillageTraded = false;

    internal void Reset(float distanceToStartingCell)
    {
        hasVillageTraded = false;

        CartographyLevelSO currentCartographyLevel = DemoController.Instance.GetCurrentCartograhpyLevel();

        if (distanceToStartingCell == currentCartographyLevel.CartographyRadius)
        {
            if (!fow.Equals(EnumFogOfWar.Undiscovered))
            {
                fow = EnumFogOfWar.Fog;
            }
        }
        else if (distanceToStartingCell > currentCartographyLevel.CartographyRadius)
        {
            // cell is too far from center to be recorded by cartograph
            fow = EnumFogOfWar.Undiscovered;
        }
    }
}