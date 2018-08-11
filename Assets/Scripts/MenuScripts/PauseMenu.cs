using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameStart))]
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private GameObject ball;

    private GameStart countdownManager;

    private LevelManager levelManager;

    private Vector2 ballStart, ballDirection;

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

        ball = GameObject.FindWithTag("Ball");

        if (!ball)
        {
            ballDirection = Vector2.zero;
            return;
        }

        ballStart = ball.transform.position;
        ballDirection = ball.GetComponent<Rigidbody2D>().velocity;
    }

    public void ClosePauseMenu()
    {
        Destroy(ball);
        pauseMenu.SetActive(false);
        countdownManager.ResetCountdown();

        StartCoroutine(ballDirection != Vector2.zero
            ? countdownManager.CountDown(ballStart, ballDirection)
            : countdownManager.CountDown(levelManager.levelCenter));
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
        RoundOver.ClearAllStaticVariables();
        SceneManager.LoadScene("ChooseControls");
    }
}