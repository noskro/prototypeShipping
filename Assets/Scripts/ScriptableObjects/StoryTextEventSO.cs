using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StoryTextEvent", order = 3)]
public class StoryTextEventSO: ScriptableObject
{
    public string Title;
    public List<string> Texts;

    public List<StoryTextEventTrigger> StoryTextEventTrigger;
    // TODO some reward for the storytextevent? new island, gold, whatever
    public List<StoryTextEventRewards> StoryTextEventRewards;

    public List<StoryTextEventSO> FollowingStoryTextEvents;

}