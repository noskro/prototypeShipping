﻿
using System;
using UnityEngine;

public class GameMapData: MonoBehaviour
{
    public EnumFogOfWar fow = EnumFogOfWar.Undiscovered;
    public bool hasVillageTraded = false;

    internal void newRun()
    {
        hasVillageTraded = false;
    }
}