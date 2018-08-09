using System;
using System.Collections;
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

[RequireComponent(typeof(ButtonIcons))]
public class ChooseControls : MonoBehaviour
{
    [SerializeField] private Text[] controlFields;

    [SerializeField] private SelectionBall selectionBall;
    [SerializeField] private Transform[] squares;

    [SerializeField] private Sprite arrow;

    public delegate void PlayerChange(PlayerColors selectedPlayer);

    public static event PlayerChange PlayerAmountIncreased, PlayerAmountDecreased, ForceAddPlayer;

    private ButtonIcons buttonIcons;

    private bool choosingLeftControl = true, selectingControls, firstPlayerSelected;
    public static bool gameStarted;

    private int numberInput;

    public static PlayerColors previouslyClearedPlayer;
    private PlayerColors selectedPlayer;

    private KeyCode pressedKey;

    private static readonly Dictionary<PlayerColors, Text[]> controlTexts = new Dictionary<PlayerColors, Text[]>();
    private readonly Dictionary<PlayerColors, float> selectionYVals = new Dictionary<PlayerColors, float>();
    public static readonly Dictionary<PlayerColors, bool> activatedPlayers = new Dictionary<PlayerColors, bool>();
    public static readonly Dictionary<PlayerColors, PlayerControls> controls = new Dictionary<PlayerColors, PlayerControls>();

    private DirectionArrows arrowManager;

    public class PlayerControls
    {
        public KeyCode leftKey;
        public KeyCode rightKey;
    }

    private void Awake()
    {
        Points.Setup();

        buttonIcons = GetComponent<ButtonIcons>();

        arrowManager = GetComponent<DirectionArrows>();

        TryAddDictionaryValues();
    }

    private void TryAddDictionaryValues()
    {
        foreach (PlayerColors item in Enum.GetValues(typeof(PlayerColors)))
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
        else if (Input.GetKeyDown(KeyCode.Space) && choosingLeftControl && activatedPlayers.Count(p => p.Value) > 1) // Left key can't be space and there must be two or more players
        {
            StartGame();
        }
        else if (KeyManagerTest.GetValidInput(ref pressedKey))
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

            if (previouslyClearedPlayer == selectedPlayer && firstPlayerSelected)
            {
                ForceAddPlayer?.Invoke(selectedPlayer);
            }
            else
            {
                firstPlayerSelected = true;
            }

            activatedPlayers[selectedPlayer] = true;
            BotSelection.botDifficulties[selectedPlayer] = 0;
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
    private void SetButton(PlayerColors playerToSet, Direction direction, KeyCode input)
    {
        buttonIcons.SetButtonIcon(input, controlTexts[playerToSet][(int)direction],
            squares[(int)playerToSet * 2 + (int)direction].GetChild(0).gameObject.GetComponent<SpriteRenderer>());

        if (direction == Direction.left)
        {
            controls[playerToSet].leftKey = input;

            arrowManager.SwitchArrowDirection();
        }
        else
        {
            controls[playerToSet].rightKey = input;
            arrowManager.RemoveArrow();
        }

        // Set ChoosingLeftControl to true if right control was just set
        choosingLeftControl = direction == Direction.right;
    }

    public static void UpdatePlayerControlText(PlayerColors player)
    {
        controlTexts[player][0].text = controls[player].leftKey.ToString();
        controlTexts[player][1].text = controls[player].rightKey.ToString();
    }

    public void ClearControls(PlayerColors playerToWipe)
    {
        SetButton(playerToWipe, Direction.left, KeyCode.None);
        SetButton(playerToWipe, Direction.right, KeyCode.None);

        controlTexts[playerToWipe][0].text = "";
        controlTexts[playerToWipe][1].text = "";

        squares[(int)playerToWipe * 2].GetChild(0).gameObject.SetActive(true);
        squares[(int)playerToWipe * 2 + 1].GetChild(0).gameObject.SetActive(true);

        squares[(int)playerToWipe * 2].GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = arrow;
        squares[(int)playerToWipe * 2 + 1].GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = arrow;

        PlayerAmountDecreased?.Invoke(playerToWipe);

        previouslyClearedPlayer = playerToWipe;
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

    public void GoToNextPlayer()
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

        PlayerAmountIncreased?.Invoke(selectingPlayer);
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
        LevelManager.isCircle = false; // Reset specific static variables from the menu level TODO these might have to be reset in other places if menu level is to change from circle to polygon
        LevelManager.shouldLerpToCircle = false;

        // TODO check if all players has had their controls set and discard those who hasn't

        gameStarted = true;
        SceneManager.LoadScene("Main");
    }
}