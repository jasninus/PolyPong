using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotSelection : MonoBehaviour
{
    public static Dictionary<PlayerColors, int> botDifficulties = new Dictionary<PlayerColors, int>();

    [SerializeField] private PlayerColors botColor;

    private ChooseControls chooseControls;

    private void Awake()
    {
        chooseControls = GameObject.FindWithTag("LevelHandler").GetComponent<ChooseControls>();

        if (botDifficulties.Count > 0) // REASON this method gets called on multiple instances, but it should only add values once
            return;

        foreach (PlayerColors color in Enum.GetValues(typeof(PlayerColors)))
        {
            botDifficulties.Add(color, 0);
        }
    }

    /// <param name="difficulty">0 = disabled, 1 = easy, 2 = medium, 3 = hard</param>
    public void SetBotDifficulty(int difficulty)
    {
        chooseControls.ClearControls(botColor);
        ChooseControls.activatedPlayers[botColor] = true;
        botDifficulties[botColor] = difficulty;
        chooseControls.GoToNextPlayer();
    }
}