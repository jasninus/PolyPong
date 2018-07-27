using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PlayerColors
{
    Yellow,
    Orange,
    Red,
    Purple,
    Blue,
    Green
}

public enum Direction
{
    left,
    right,
    middle
}

public class ChooseControls : MonoBehaviour
{
    [SerializeField] private Text[] controlFields;

    [SerializeField] private SelectionBall selectionBall;
    [SerializeField] private Transform[] squares;

    private bool choosingLeftControl = true, selectingControls;

    private int numberInput;

    private PlayerColors selectedPlayer;

    private static readonly Dictionary<PlayerColors, Text[]> controlTexts = new Dictionary<PlayerColors, Text[]>();
    private readonly Dictionary<PlayerColors, float> selectionYVals = new Dictionary<PlayerColors, float>();
    public static readonly Dictionary<PlayerColors, bool> activatedPlayers = new Dictionary<PlayerColors, bool>();
    public static readonly Dictionary<PlayerColors, PlayerControls> controls = new Dictionary<PlayerColors, PlayerControls>();

    public class PlayerControls
    {
        public string leftKey;
        public string rightKey;
    }

    private void Awake()
    {
        Points.Setup();

        TryAddDictionaryValues();
    }

    private void TryAddDictionaryValues()
    {
        foreach (PlayerColors item in Enum.GetValues(typeof(PlayerColors))) // TODO check if key is present in dictionary before adding
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
            activatedPlayers.Add(item, false);

            // Add new PlayerControls for all colors
            controls.Add(item, new PlayerControls());
        }
    }

    private void Start()
    {
        SetSelectedPlayer(PlayerColors.Yellow);
    }

    private void Update()
    {
        // Should inputs change selected player or be used as controls
        if (!selectingControls && GetDigitInput(out numberInput))
        {
            SetPlayerWithDigit(numberInput);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearPlayer(selectedPlayer);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && choosingLeftControl && activatedPlayers.Count(p => p.Value) > 1) // Space should not be allowed as left key
        {
            StartGame();
        }
        else if (LookForAcceptableInput())
        {
            SetControls();
        }
    }

    private void SetControls()
    {
        selectingControls = true;

        if (choosingLeftControl) // Set leftKey control
        {
            SetButton(selectedPlayer, Direction.left, Input.inputString);
            activatedPlayers[selectedPlayer] = true;
            BotSelection.botDifficulties[selectedPlayer] = 0;
        }
        else // Set rightKey control
        {
            SetButton(selectedPlayer, Direction.right, Input.inputString);
            selectingControls = false;

            GoToNextPlayer();
        }
    }

    /// <summary>
    /// Set text and control for selectedPlayer and direction to input
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="input"></param>
    private void SetButton(PlayerColors playerToSet, Direction direction, string input)
    {
        controlTexts[playerToSet][0 + (int)direction].text = input;
        squares[(int)playerToSet * 2 + (int)direction].GetChild(0).gameObject.SetActive(false);

        if (direction == Direction.left)
            controls[playerToSet].leftKey = input;
        else
            controls[playerToSet].rightKey = input;

        // Set ChoosingLeftControl to true if right control was just set
        choosingLeftControl = direction == Direction.right;
    }

    public static void UpdatePlayerControlText(PlayerColors player)
    {
        controlTexts[player][0].text = controls[player].leftKey;
        controlTexts[player][1].text = controls[player].rightKey;
    }

    public void ClearControls(PlayerColors playerToWipe)
    {
        SetButton(playerToWipe, Direction.left, "");
        SetButton(playerToWipe, Direction.right, "");

        squares[(int)playerToWipe * 2].GetChild(0).gameObject.SetActive(true);
        squares[(int)playerToWipe * 2 + 1].GetChild(0).gameObject.SetActive(true);
    }

    private void ClearPlayer(PlayerColors playerToWipe)
    {
        ClearControls(playerToWipe);

        activatedPlayers[playerToWipe] = false;
        BotSelection.botDifficulties[playerToWipe] = 0;
        selectingControls = false;
    }

    /// <summary>
    /// Returns true if current input is acceptable as control for players
    /// </summary>
    private bool LookForAcceptableInput()
    {
        return Input.inputString.Length == 1;
    }

    private void GoToNextPlayer()
    {
        try
        {
            PlayerColors pc = activatedPlayers.First(item => !item.Value).Key;
            SetSelectedPlayer(pc);
        }
        catch { }
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
        SetSelectedPlayer((PlayerColors)playerNumber - 1);
    }

    private void SetSelectedPlayer(PlayerColors selectingPlayer)
    {
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
        SetSelectedPlayer((PlayerColors)selectingPlayer);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
}