using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundOver : MonoBehaviour
{
    public delegate void Round();

    public static event Round RoundEnd;

    private static int roundsPlayed;

    private Text winText;

    private GameObject panel;

    private bool gameEnded;

    private void Awake()
    {
        CircleCollider.RoundWin += EndRound;
    }

    public void EndRound()
    {
        if (roundsPlayed >= RoundAmountRenumeration.selectedRoundAmount - 1)
        {
            CreateVictoryScreen();
        }
        else
        {
            CreateNextRoundScreen();
        }

        gameEnded = true;
    }

    private void CreateNextRoundScreen()
    {
        GameObject panel = GameObject.FindWithTag("Panel");
        panel.GetComponent<Image>().enabled = true;
        panel.GetComponent<Image>().color = Color.gray;
        GameObject.FindWithTag("WinText").GetComponent<Text>().text = "Press space to continue";

        foreach (Player backupPlayer in PlayerManager.backupPlayers)
        {
            ChooseControls.playerStates[backupPlayer.Color] = PlayerState.Activated;
        }
    }

    private void CreateVictoryScreen()
    {
        GameObject panel = GameObject.FindWithTag("Panel");
        panel.GetComponent<Image>().enabled = true;
        panel.GetComponent<Image>().color = MeshManager.materials[Points.GetWinner()].color;
        GameObject.FindWithTag("WinText").GetComponent<Text>().text = Points.GetWinner() + " player has won!";
        // TODO display final scores
    }

    public static void ClearAllStaticVariables()
    {
        ChooseControls.playerStates.Clear();
        ChooseControls.controls.Clear();
        PlayerManager.players.Clear();
        MeshManager.materials.Clear();
        PlayerManager.backupPlayers.Clear();
        InGameManager.isCircle = false;
        InGameManager.shouldLerpToCircle = false;
        ChooseControls.gameStarted = false;
    }

    public static void ResetStaticVariables()
    {
        MeshManager.materials.Clear();
        PlayerManager.players.Clear();
        InGameManager.isCircle = false;
        InGameManager.shouldLerpToCircle = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gameEnded)
        {
            Points.previousPlayerDeaths = 0;
            roundsPlayed++;

            if (roundsPlayed < RoundAmountRenumeration.selectedRoundAmount) // More rounds remaining
            {
                ResetStaticVariables();
                SceneManager.LoadScene(2);
            }
            else // Last round done
            {
                Points.ResetPoints();
                ClearAllStaticVariables();
                roundsPlayed = 0;
                SceneManager.LoadScene(1);
                RoundAmountRenumeration.selectedRoundAmount = 1;
            }
        }
    }
}