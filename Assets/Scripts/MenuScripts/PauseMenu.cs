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

    private Vector2[] ballStarts, ballDirections;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
        countdownManager = GetComponent<GameStart>();
        GameStart.BallSpawn += SpawnExtraBalls;
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

        if (!balls[0])
        {
            ballDirections[0] = Vector2.zero;
            return;
        }

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

        StartCoroutine(ballDirections[0] != Vector2.zero
            ? countdownManager.CountDown(ballStarts[0], ballDirections[0])
            : countdownManager.CountDown(levelManager.levelCenter)); // Game not started before pause
    }

    private void SpawnExtraBalls(GameObject spawnedBall)
    {
        if (balls?.Length > 1)
        {
            for (int i = 1; i < balls.Length; i++)
            {
                GameObject ball = Instantiate(spawnedBall, ballStarts[i], Quaternion.identity);
                ball.GetComponent<Rigidbody2D>().velocity = ballDirections[i];
            }

            balls = null;
        }
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
        RoundOver.ClearAllStaticVariables();
        SceneManager.LoadScene("ChooseControls");
    }
}