using System;

[Serializable]
public class StoryTextEventRewards
{
    public EnumIslandUnlockEvent unlockIslands;

    public EnumEventModifierRewardType rewardType;
    public int intValue;

    public EnumStoryProgressionTags progressionTag;
    //public List< CityDataSO revelantCity;
    //public EnumGameStates nextGameState;
}