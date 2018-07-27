﻿using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundOver : MonoBehaviour
{
    private static int roundsPlayed;

    private Text winText;

    private GameObject panel;

    private bool gameEnded;

    private void Awake()
    {
        CircleCollider.RoundWin += CreateVictoryScreen;
    }

    public void CreateVictoryScreen(Player winner)
    {
        GameObject panel = GameObject.FindWithTag("Panel");
        panel.GetComponent<Image>().enabled = true;
        panel.GetComponent<Image>().color = MeshManager.materials[winner.color].color;
        GameObject.FindWithTag("WinText").GetComponent<Text>().text = winner.color + " player has won!";
        gameEnded = true;
    }

    private void ClearAllStaticVariables()
    {
        ChooseControls.activatedPlayers.Clear();
        ChooseControls.controls.Clear();
        PlayerManager.players.Clear();
        MeshManager.materials.Clear();
        LevelManager.isCircle = false;
        LevelManager.shouldLerpToCircle = false;
    }

    private void NewRoundFixStaticVariables()
    {
        MeshManager.materials.Clear();
        PlayerManager.players.Clear();
        LevelManager.isCircle = false;
        LevelManager.shouldLerpToCircle = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gameEnded == true)
        {
            roundsPlayed++;

            if (roundsPlayed < RoundSlider.selectedRoundAmount)
            {
                // TODO when spawning from circle, it will revert to triangle
                // TODO seems like controls while not in circle is lost
                NewRoundFixStaticVariables();
                SceneManager.LoadScene(2);
            }
            else
            {
                ClearAllStaticVariables();
                roundsPlayed = 0;
                SceneManager.LoadScene(1);
            }
        }
    }
}