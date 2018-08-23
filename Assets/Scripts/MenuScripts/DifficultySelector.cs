using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySelector : ValueRenumeration
{
    [SerializeField] private float ballSpeedModifierEasy, ballSpeedModifierMedium, ballSpeedModifierHard;

    public void ChangeDifficulty(int difficultyChange)
    {
        int val = Mathf.RoundToInt(RenumerateWithClamp(difficultyChange));

        switch (val)
        {
            case -1:
                text.text = "Easy";
                Ball.ballSpeedModifier = ballSpeedModifierEasy;
                break;

            case 0:
                text.text = "Medium";
                Ball.ballSpeedModifier = ballSpeedModifierMedium;
                break;

            case 1:
                text.text = "Hard";
                Ball.ballSpeedModifier = ballSpeedModifierHard;
                break;
        }
    }
}