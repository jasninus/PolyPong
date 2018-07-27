using UnityEngine.UI;
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

    private void ResetStaticVariables()
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

            Points.previousPlayerDeaths = 0;

            if (roundsPlayed < RoundSlider.selectedRoundAmount)
            {
                ResetStaticVariables();
                SceneManager.LoadScene(2);
            }
            else
            {
                Debug.Log(Points.GetWinner() + " has actually won");
                Points.ResetPoints();
                ClearAllStaticVariables();
                roundsPlayed = 0;
                SceneManager.LoadScene(1);
            }
        }
    }
}