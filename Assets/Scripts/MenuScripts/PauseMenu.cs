using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameStart))]
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private GameObject[] balls;

    private GameStart countdownManager;

    private LevelManager levelManager;

    private Vector2[] ballStarts = new Vector2[0], ballDirections = new Vector2[0];

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
        countdownManager = GetComponent<GameStart>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchPauseMenu();
        }
    }

    private void SwitchPauseMenu()
    {
        Invoke(pauseMenu.activeInHierarchy ? "ClosePauseMenu" : "OpenPauseMenu", 0);
    }

    private void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;

        balls = GameObject.FindGameObjectsWithTag("Ball");

        ballStarts = balls.Select(b => (Vector2)b.transform.position).ToArray();
        ballDirections = balls.Select(b => b.GetComponent<Rigidbody2D>().velocity).ToArray();
    }

    public void ClosePauseMenu()
    {
        foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball"))
        {
            Destroy(ball);
        }

        pauseMenu.SetActive(false);
        countdownManager.ResetCountdown();

        StartCoroutine(ballDirections.Length > 0
            ? countdownManager.CountDown(ballStarts, ballDirections)
            : countdownManager.CountDown(levelManager.levelCenter)); // Game not started before pause
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
        RoundOver.ClearAllStaticVariables();
        SceneManager.LoadScene("ChooseControls");
    }
}