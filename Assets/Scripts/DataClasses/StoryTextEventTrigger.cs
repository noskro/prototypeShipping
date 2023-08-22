using System;

[Serializable]
public class StoryTextEventTrigger
{
    public EnumStoryTextEventTriggerType type;    
    public int intValue;
    public CityDataSO revelantCity;
    public EnumGameStates nextGameState;
}

