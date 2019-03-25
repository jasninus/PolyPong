using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotDifficultySetter : MonoBehaviour
{
    [SerializeField] private PlayerColor color;

    /// <summary>
    /// Sets a player state to bot difficulty based on parameter
    /// </summary>
    /// <param name="difficulty">1 for easy, 2 for medium, 3 for hard</param>
    public void SetDifficulty(int difficulty)
    {
        if (1 > difficulty || difficulty > 3)
        {
            Debug.LogError("Wrong input for bot difficulty selection. Must 1, 2 or 3 (int)");
            return;
        }

        ChooseControls.playerStates[color] = (PlayerState)difficulty;
    }
}