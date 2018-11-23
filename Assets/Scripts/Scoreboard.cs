using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private float startYPos, yDiff;

    private readonly Dictionary<PlayerColor, Text> pointTexts = new Dictionary<PlayerColor, Text>();

    private void Start()
    {
        if (pointTexts.Count > 0)
            pointTexts.Clear();

        foreach (PlayerColor color in Enum.GetValues(typeof(PlayerColor)))
        {
            if (ChooseControls.playerStates[color] != PlayerState.Deactivated)
            {
                transform.GetChild((int)color).gameObject.SetActive(true);
                pointTexts.Add(color, transform.GetChild((int)color).GetComponent<Text>());
            }
        }

        RoundOver.RoundEnd += UpdateScoreboard;
        Player.UpdateScoreboard += UpdateText;
        UpdateScoreboard();
    }

    private void UpdateScoreboard()
    {
        UpdatePositions();
        UpdateText();
    }

    private void UpdatePositions()
    {
        List<PlayerColor> colorOrder = Points.GetSortedPlayers();

        foreach (var pointText in pointTexts)
        {
            float yPos = startYPos - yDiff * colorOrder.IndexOf(pointText.Key);
            pointText.Value.transform.position = new Vector3(pointText.Value.transform.position.x, yPos, 0);
        }
    }

    private void UpdateText()
    {
        foreach (var text in pointTexts)
        {
            text.Value.text = Points.points[text.Key].ToString();
        }
    }

    private void OnDestroy()
    {
        Player.UpdateScoreboard -= UpdateText;
    }
}