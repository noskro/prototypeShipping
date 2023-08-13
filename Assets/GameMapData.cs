
using System;
using UnityEngine;

public class GameMapData
{
    public EnumFogOfWar fow = EnumFogOfWar.Undiscovered;
    public bool hasVillageTraded = false;

    internal void Reset()
    {
        hasVillageTraded = false;
    }
}