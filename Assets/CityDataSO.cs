using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CityDataSO", order = 3)]
public class CityDataSO: ScriptableObject
{
    public string CityName;

    public int startingCityLevel;

}