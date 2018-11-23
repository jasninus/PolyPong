using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal.Input;

public static class Points
{
    public static readonly Dictionary<PlayerColor, int> points = new Dictionary<PlayerColor, int>();

    public static int previousPlayerDeaths;

    public static void Setup()
    {
        if (points.Count > 0)
            return;

        foreach (PlayerColor color in Enum.GetValues(typeof(PlayerColor)))
        {
            points.Add(color, 0);
        }
    }

    public static void AddPoints(PlayerColor color)
    {
        points[color] += previousPlayerDeaths;
        previousPlayerDeaths++;
    }

    public static void ResetPoints()
    {
        foreach (PlayerColor color in Enum.GetValues(typeof(PlayerColor)))
        {
            points[color] = 0;
        }
    }

    public static PlayerColor GetWinner() // TODO tie functionality must be implemented
    {
        PlayerColor winner = PlayerColor.Yellow;

        foreach (PlayerColor color in Enum.GetValues(typeof(PlayerColor)))
        {
            if (points[color] > points[winner])
            {
                winner = color;
            }
        }

        return winner;
    }

    public static List<PlayerColor> GetSortedPlayers()
    {
        return points.OrderByDescending(i => i.Value).Select(i => i.Key).ToList();
    }
}