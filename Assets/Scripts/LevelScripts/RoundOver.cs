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
        if (roundsPlayed >= RoundSlider.selectedRoundAmount - 1)
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
    }

    private void CreateVictoryScreen()
    {
        GameObject panel = GameObject.FindWithTag("Panel");
        panel.GetComponent<Image>().enabled = true;
        panel.GetComponent<Image>().color = MeshManager.materials[Points.GetWinner()].color;
        GameObject.FindWithTag("WinText").GetComponent<Text>().text = Points.GetWinner() + " player has won!";
    }

    public static void ClearAllStaticVariables()
    {
        ChooseControls.activatedPlayers.Clear();
        ChooseControls.controls.Clear();
        PlayerManager.players.Clear();
        MeshManager.materials.Clear();
        PlayerManager.backupPlayers.Clear();
        LevelManager.isCircle = false;
        LevelManager.shouldLerpToCircle = false;
        ChooseControls.gameStarted = false;
        MenuLevelManager.levelIsSpawned = false;
    }

    public static void ResetStaticVariables()
    {
        MeshManager.materials.Clear();
        PlayerManager.players.Clear();
        LevelManager.isCircle = false;
        LevelManager.shouldLerpToCircle = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gameEnded)
        {
            Points.previousPlayerDeaths = 0;
            roundsPlayed++;

            if (roundsPlayed < RoundSlider.selectedRoundAmount) // More rounds remaining
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
                RoundSlider.selectedRoundAmount = 1;
            }
        }
    }
}