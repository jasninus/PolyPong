using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerCount : MonoBehaviour
{
    [SerializeField]
    private Text text;

    [SerializeField]
    private Text text1;

    [SerializeField]
    private int sceneToLoad;

    private int numberOfPlayers;

    private void Start()
    {
        text.text = "How many players are you?";

        text1.text = "Press 2,3,4,5 or 6";
    }

    private void Update()
    {
        if (Input.GetKey("2"))
        {
            numberOfPlayers = 2;
            PlayerPrefs.SetInt("Number of players", numberOfPlayers);
            SceneManager.LoadScene(sceneToLoad);
        }
        if (Input.GetKeyDown("3"))
        {
            numberOfPlayers = 3;
            PlayerPrefs.SetInt("Number of players", numberOfPlayers);
            SceneManager.LoadScene(sceneToLoad);
        }
        if (Input.GetKeyDown("4"))
        {
            numberOfPlayers = 4;
            PlayerPrefs.SetInt("Number of players", numberOfPlayers);
            SceneManager.LoadScene(sceneToLoad);
        }
        if (Input.GetKeyDown("5"))
        {
            numberOfPlayers = 5;
            PlayerPrefs.SetInt("Number of players", numberOfPlayers);
            SceneManager.LoadScene(sceneToLoad);
        }
        if (Input.GetKeyDown("6"))
        {
            numberOfPlayers = 6;
            PlayerPrefs.SetInt("Number of players", numberOfPlayers);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void FixedUpdate()
    {
    }
}