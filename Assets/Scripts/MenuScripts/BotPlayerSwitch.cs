using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BotPlayerSwitch : MonoBehaviour
{
    [SerializeField] private GameObject botButtonsParent, playerSquaresParent;

    [SerializeField] private PlayerColor color;

    [SerializeField] private Sprite botSprite, playerSprite;

    [SerializeField] private MenuLevel menuLevel;

    private Image image;

    private ChooseControls chooseControls;

    private void Awake()
    {
        image = GetComponent<Image>();
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

        ChooseControls.playerStates[color] = PlayerState.BotMedium;

        Debug.Log("Adding " + color + " bot");
        //menuLevel.AddQueueItem(new MenuLevel.QueueItem { color = color, lerpType = true });

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