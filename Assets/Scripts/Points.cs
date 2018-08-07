using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal.Input;

public static class Points
{
    public static readonly Dictionary<PlayerColors, int> points = new Dictionary<PlayerColors, int>();

    public static int previousPlayerDeaths;

    public static void Setup()
    {
        if (points.Count > 0)
            return;

        foreach (PlayerColors color in Enum.GetValues(typeof(PlayerColors)))
        {
            points.Add(color, 0);
        }
    }

    public static void AddPoints(PlayerColors color)
    {
        points[color] += previousPlayerDeaths;
        previousPlayerDeaths++;
    }

    public static void ResetPoints()
    {
        foreach (PlayerColors color in Enum.GetValues(typeof(PlayerColors)))
        {
            points[color] = 0;
        }
    }

    public static PlayerColors GetWinner() // TODO tie functionality must be implemented
    {
        PlayerColors winner = PlayerColors.Yellow;

        foreach (PlayerColors color in Enum.GetValues(typeof(PlayerColors)))
        {
            if (points[color] > points[winner])
            {
                winner = color;
            }
        }

        return winner;
    }

    public static List<PlayerColors> GetSortedPlayers()
    {
        return points.OrderByDescending(i => i.Value).Select(i => i.Key).ToList();
    }
}