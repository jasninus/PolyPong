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
        chooseControls = GameObject.FindWithTag("ControlManager").GetComponent<ChooseControls>();

        if (botDifficulties.Count > 0) // This method gets called on multiple instances, but it should only add values once
            return;

        foreach (PlayerColors color in Enum.GetValues(typeof(PlayerColors)))
        {
            botDifficulties.Add(color, 0); 
        }
    }

    public void SetBotDifficulty(int difficulty)
    {
        chooseControls.ClearControls(botColor);
        ChooseControls.activatedPlayers[botColor] = true;
        botDifficulties[botColor] = difficulty;
        Debug.Log(botColor + " was set to difficulty: " + difficulty);
    }
}