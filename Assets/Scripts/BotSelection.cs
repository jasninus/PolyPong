using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO this class should be placed on the object that contains the three bot difficulty buttons for each player
public class BotSelection : MonoBehaviour
{
    public static Dictionary<PlayerColors, Bot> bots;

    public class Bot
    {
        public bool activated;
        public int difficulty;
    }

    [SerializeField] private PlayerColors color;

    private void AddDictionaryValues()
    {
        foreach (PlayerColors item in Enum.GetValues(typeof(PlayerColors)))
        {
            // Add all text vars to controlTexts
            bots.Add(item, new Bot());
        }
    }

    public void SelectBotDifficulty(int difficulty) // Run this when button is clicked
    {
        bots[color].activated = true;
        bots[color].difficulty = difficulty;
    }
}
