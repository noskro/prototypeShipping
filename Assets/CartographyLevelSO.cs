
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CartographyLevel", order = 2)]
public class CartographyLevelSO : ScriptableObject
{
    public string CartographyLevelName;

    public int UpgradePrice; // how much damage the ship can take
    public int CartographyRadius;



}