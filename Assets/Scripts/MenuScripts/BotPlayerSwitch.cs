using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BotPlayerSwitch : MonoBehaviour
{
    [SerializeField] private GameObject botButtonsParent, playerSquaresParent;

    [SerializeField] private PlayerColors color;

    [SerializeField] private Sprite botSprite, playerSprite;

    private BotSelection botSelection;

    private Image image;

    private ChooseControls chooseControls;

    private void Awake()
    {
        image = GetComponent<Image>();
        botSelection = botButtonsParent.GetComponent<BotSelection>();
        chooseControls = GameObject.FindWithTag("LevelHandler").GetComponent<ChooseControls>();
    }

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

        botSelection.SetBotDifficulty(2);

        image.sprite = botSprite;
    }

    private void SwitchToPlayer()
    {
        playerSquaresParent.SetActive(true);
        botButtonsParent.SetActive(false);

        chooseControls.ClearControls(color);

        image.sprite = playerSprite;
    }
}