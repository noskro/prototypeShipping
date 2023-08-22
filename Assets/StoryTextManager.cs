using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTextManager : MonoBehaviour
{
    public StoryTextUI storyTextUI;

    public List<StoryTextEventSO> ActiveStoryTextEventList;
    public List<StoryTextEventSO> CompletedStoryTextEventList;

    private EnumGameStates currentGameState;

    private void Start()
    {
        DemoController.OnGameStateChanged += CheckForNewEvents;

        StoryTextUI.OnStoryTextCompleted += CheckForNewEvents;
    }

    public void CheckForNewEvents()
    {
        CheckForNewEvents(currentGameState);
    }

    public void CheckForNewEvents(EnumGameStates gameState)
    {
        currentGameState = gameState;

        foreach (StoryTextEventSO story in ActiveStoryTextEventList)
        {
            foreach (StoryTextEventTrigger trigger in story.StoryTextEventTrigger)
            {
                if (trigger.type.Equals(EnumStoryTextEventTriggerType.Now) ||
                    (gameState.Equals(EnumGameStates.Start) && trigger.type.Equals(EnumStoryTextEventTriggerType.NextRun)) ||
                    (trigger.type.Equals(EnumStoryTextEventTriggerType.Round) && (int)trigger.intValue >= DemoController.Instance.Round) ||
                    (trigger.type.Equals(EnumStoryTextEventTriggerType.DiscoverCity) && StaticTileDataContainer.Instance.IsCityDiscovered(trigger.revelantCity)) ||
                    (trigger.type.Equals(EnumStoryTextEventTriggerType.NextGameState) && trigger.nextGameState.Equals(DemoController.Instance.GameState)) ||
                    false) // false is here just so I can copy the condition lines without caring about the || at the end
                {
                    storyTextUI.SetStoryText(story);

                    ActiveStoryTextEventList.AddRange(story.FollowingStoryTextEvents);

                    CompletedStoryTextEventList.Add(story);
                    ActiveStoryTextEventList.Remove(story);
                    return;
                }
            }
        }
    }
}