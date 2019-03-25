using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PlayerColor
{
    Yellow,
    Orange,
    Red,
    Purple,
    Blue,
    Green
}

// Don't change the order. Is used for casting to and from int
public enum PlayerState
{
    Deactivated,
    BotEasy,
    BotMedium,
    BotHard,
    Activated,
}

public enum Direction
{
    left,
    right,
    middle
}

[RequireComponent(typeof(ButtonIcons))]
public class ChooseControls : MonoBehaviour
{
    [SerializeField] private Text[] controlFields;

    [SerializeField] private SelectionBall selectionBall;
    [SerializeField] private Transform[] squares;

    [SerializeField] private Sprite arrow;

    [SerializeField] private MenuLevel menuLevel;

    private ButtonIcons buttonIcons;

    private bool choosingLeftControl = true, selectingControls, firstPlayerSelected;
    public static bool gameStarted;

    private int numberInput;

    public static PlayerColor previouslyClearedPlayer;
    private PlayerColor selectedPlayer;

    private KeyCode pressedKey;

    private static readonly Dictionary<PlayerColor, Text[]> controlTexts = new Dictionary<PlayerColor, Text[]>();
    private readonly Dictionary<PlayerColor, float> selectionYVals = new Dictionary<PlayerColor, float>();
    public static readonly Dictionary<PlayerColor, PlayerState> playerStates = new Dictionary<PlayerColor, PlayerState>();
    public static readonly Dictionary<PlayerColor, PlayerControls> controls = new Dictionary<PlayerColor, PlayerControls>();

    [SerializeField] private ControlsMenuSwitcher menuSwitcher;

    public class PlayerControls
    {
        public KeyCode leftKey;
        public KeyCode rightKey;
    }

    private void Awake()
    {
        Points.Setup();
        buttonIcons = GetComponent<ButtonIcons>();
        TryAddDictionaryValues();
    }

    private void TryAddDictionaryValues()
    {
        foreach (PlayerColor item in Enum.GetValues(typeof(PlayerColor)))
        {
            if (controlTexts.ContainsKey(item) && controlTexts[item][0] == null)
            {
                controlTexts.Remove(item);
            }

            if (!controlTexts.ContainsKey(item))
            {
                // Add all text vars to controlTexts
                controlTexts.Add(item, new Text[] { controlFields[(int)item * 2], controlFields[(int)item * 2 + 1] });
            }

            // Add all heights for the selectionBall
            selectionYVals.Add(item, squares[(int)item * 2].position.y);

            // Add all player colors
            playerStates.Add(item, PlayerState.Deactivated);

            // Add new PlayerControls for all colors
            controls.Add(item, new PlayerControls());
        }
    }

    private void Start()
    {
        SetSelectedPlayer(PlayerColor.Yellow);
    }

    private void Update()
    {
        // Don't do anything if this is not the currently active menu
        if (!menuSwitcher.IsCurrentMenu) { return; }

        // Should inputs change selected player or be used as controls
        if (!selectingControls && GetDigitInput(out numberInput))
        {
            SetPlayerWithDigit(numberInput);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearPlayer(selectedPlayer);
        }
        // Left key can't be space and there must be two or more players to start the game
        else if (Input.GetKeyDown(KeyCode.Space) && choosingLeftControl && playerStates.Count(p => p.Value != PlayerState.Deactivated) > 1)
        {
            StartGame();
        }
        else if (InputManager.GetValidInput(ref pressedKey))
        {
            SetControls(pressedKey);
        }
    }

    private void SetControls(KeyCode key)
    {
        selectingControls = true;

        if (choosingLeftControl) // Set leftKey control
        {
            SetButton(selectedPlayer, Direction.left, key);
            playerStates[selectedPlayer] = PlayerState.Activated;
        }
        else // Set rightKey control
        {
            SetButton(selectedPlayer, Direction.right, key);
            selectingControls = false;

            GoToNextPlayer();
        }
    }

    /// <summary>
    /// Set text and control for selectedPlayer and direction to input
    /// </summary>
    private void SetButton(PlayerColor playerToSet, Direction direction, KeyCode input)
    {
        buttonIcons.SetButtonIcon(input, controlTexts[playerToSet][(int)direction],
            squares[(int)playerToSet * 2 + (int)direction].GetChild(0).gameObject.GetComponent<Image>());

        if (direction == Direction.left)
        {
            controls[playerToSet].leftKey = input;
        }
        else
        {
            controls[playerToSet].rightKey = input;
        }

        // Set ChoosingLeftControl to true if right control was just set
        choosingLeftControl = direction == Direction.right;
    }

    public static void UpdatePlayerControlText(PlayerColor player)
    {
        controlTexts[player][0].text = controls[player].leftKey.ToString();
        controlTexts[player][1].text = controls[player].rightKey.ToString();
    }

    public void ClearControls(PlayerColor playerToWipe)
    {
        SetButton(playerToWipe, Direction.left, KeyCode.None);
        SetButton(playerToWipe, Direction.right, KeyCode.None);

        controlTexts[playerToWipe][0].text = "";
        controlTexts[playerToWipe][1].text = "";

        squares[(int)playerToWipe * 2].GetChild(0).gameObject.SetActive(true);
        squares[(int)playerToWipe * 2 + 1].GetChild(0).gameObject.SetActive(true);

        squares[(int)playerToWipe * 2].GetChild(0).gameObject.GetComponent<Image>().sprite = arrow;
        squares[(int)playerToWipe * 2 + 1].GetChild(0).gameObject.GetComponent<Image>().sprite = arrow;

        previouslyClearedPlayer = playerToWipe;

        Debug.Log("Clearing " + playerToWipe + " player");
    }

    private void ClearPlayer(PlayerColor playerToWipe)
    {
        ClearControls(playerToWipe);
        playerStates[playerToWipe] = PlayerState.Deactivated;
        selectingControls = false;
    }

    /// <summary>
    /// Returns true if current input is acceptable as control for players
    /// </summary>
    private bool LookForAcceptableInput()
    {
        return Input.inputString.Length == 1;
    }

    public void GoToNextPlayer()
    {
        for (int i = 0; i < 6 /*Amount of PlayerColor*/; i++)
        {
            if (playerStates[(PlayerColor)i] == PlayerState.Deactivated)
            {
                SetSelectedPlayer((PlayerColor)i);
                return;
            }
        }
    }

    /// <summary>
    /// Sets param to a digit input from 1-6
    /// </summary>
    /// <param name="numberInput">Int to be set to valid input</param>
    /// <returns>True if Input.inputString returned valid input</returns>
    private bool GetDigitInput(out int numberInput)
    {
        return int.TryParse(Input.inputString, out numberInput) && numberInput <= 6 && numberInput >= 1;
    }

    /// <summary>
    /// Set currently selected player with alphanumeric keys 1-6
    /// </summary>
    private void SetPlayerWithDigit(int playerNumber)
    {
        SetSelectedPlayer((PlayerColor)playerNumber - 1);
    }

    private void SetSelectedPlayer(PlayerColor selectingPlayer)
    {
        //Debug.Log("Clearing " + selectedPlayer + " player");
        //Debug.Log("Adding " + selectingPlayer + " player"); // TODO should probably also add player to playerStates here, so that it can be used by MenuLevel
        selectionBall.StartPosLerpToPoint(new Vector3(selectionBall.transform.position.x, selectionYVals[selectingPlayer], selectionBall.transform.position.z));
        selectedPlayer = selectingPlayer;
    }

    /// <summary>
    /// Only use this for buttons, since buttons can't use enums as params
    /// </summary>
    /// <param name="selectingPlayer">The player to be selected</param>
    /// <remarks>May need to be updated in case of different player orders</remarks>
    public void ButtonSetPlayer(int selectingPlayer)
    {
        SetSelectedPlayer((PlayerColor)selectingPlayer);
    }

    private void StartGame()
    {
        InGameManager.isCircle = false;
        InGameManager.shouldLerpToCircle = false;
        gameStarted = true;
        SceneManager.LoadScene("Main");
    }
}