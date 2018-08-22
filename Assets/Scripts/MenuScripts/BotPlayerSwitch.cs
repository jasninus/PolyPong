using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotPlayerSwitch : MonoBehaviour
{
    [SerializeField] private GameObject botButtonsParent, playerSquaresParent;

    [SerializeField] private PlayerColors color;

    public void SwitchActivePlayerType()
    {
        if (botButtonsParent.activeInHierarchy)
        {
            SwitchToPlayer();
        }
        else
        {
            SwitchToBot();
        }
    }

    private void SwitchToBot()
    {
        botButtonsParent.SetActive(true);
        playerSquaresParent.SetActive(false);

        BotSelection.botDifficulties[color] = 2;
    }

    private void SwitchToPlayer()
    {
        playerSquaresParent.SetActive(true);
        botButtonsParent.SetActive(false);
    }
}