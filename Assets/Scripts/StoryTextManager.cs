using System;
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
                    (trigger.type.Equals(EnumStoryTextEventTriggerType.Run) && (int)trigger.intValue <= DemoController.Instance.Run) ||
                    (trigger.type.Equals(EnumStoryTextEventTriggerType.FieldsTravelled) && (int)trigger.intValue <= DemoController.Instance.FieldsTravelled) ||
                    (trigger.type.Equals(EnumStoryTextEventTriggerType.DiscoverCity) && StaticTileDataContainer.Instance.IsCityDiscovered(trigger.revelantCity)) ||
                    (trigger.type.Equals(EnumStoryTextEventTriggerType.TradeWithCity) && StaticTileDataContainer.Instance.IsTradedWith(trigger.revelantCity)) ||
                    (trigger.type.Equals(EnumStoryTextEventTriggerType.NextGameState) && trigger.nextGameState.Equals(DemoController.Instance.GameState)) ||
                    false) // false is here just so I can copy the condition lines without caring about the || at the end
                {
                    TriggerStoryTextEvent(story);
                    CheckForNewEvents(gameState);
                    return;
                }
            }

            if (story.StoryTextEventTrigger.Count == 0)
            {
                // if there is no trigger, akways trigger
                TriggerStoryTextEvent(story);
                CheckForNewEvents(gameState);
                return;
            }
        }
    }

    private void TriggerStoryTextEvent(StoryTextEventSO story)
    {
        storyTextUI.SetStoryText(story);

        ActiveStoryTextEventList.AddRange(story.FollowingStoryTextEvents);

        // fulllfill rewards
        foreach (StoryTextEventRewards reward in story.StoryTextEventRewards)
        {
            if (!reward.unlockIslands.Equals(EnumIslandUnlockEvent.StarterIsland))
            {
                DemoController.Instance.worldCreator.AddNewIslandPrefabsToAvailable(reward.unlockIslands);
            }

            if (!reward.rewardType.Equals(EnumEventModifierRewardType.None))
            {
                if (reward.rewardType.Equals(EnumEventModifierRewardType.MapUncover))
                {
                    DemoController.Instance.shipController.gameMapHandler.DiscoverNewAreaByShip(DemoController.Instance.gameMapHandler.shipCoordinates, 7, 5);
                }
                else
                {
                    DemoController.Instance.shipController.shipStats.AddStatsModifier(reward.rewardType, reward.intValue);
                }
            }

            if (!reward.progressionTag.Equals(EnumStoryProgressionTags.None))
            {
                if (!DemoController.Instance.StoryProgressionTags.Contains(reward.progressionTag))
                {
                    DemoController.Instance.StoryProgressionTags.Add(reward.progressionTag);
                }
            }
        }

        DemoController.Instance.shipController.TriggerShipUpdated();
        CompletedStoryTextEventList.Add(story);
        ActiveStoryTextEventList.Remove(story);
    }
}
