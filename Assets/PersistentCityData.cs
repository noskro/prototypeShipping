using System;

public class PersistentCityData
{
    public string CityName;
    public int CityLevel;

    public int TimesTraded;
    public bool HasTradedThisRun;

    public PersistentCityData(CityDataSO cityDataSo)
    {
        CityName = cityDataSo.CityName;
        CityLevel = cityDataSo.startingCityLevel;
    }

    internal void Reset()
    {
        HasTradedThisRun = false;
    }

    internal void SetTradedThisRun(bool value)
    {
        HasTradedThisRun = value;
    }
}