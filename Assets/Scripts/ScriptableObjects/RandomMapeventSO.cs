using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RandomMapEvent", order = 3)]
public partial class RandomMapEventSO : ScriptableObject
{
    public string EventName;

    public CustomTile eventTile;
    public List<CustomTile> placedOnAnyTile;

    public int EventMinOccurence;
    public int EventMaxOccurence;

    public bool EventRepeatable;

    public List<RandomEventResult> RandomEventResultList;
}
