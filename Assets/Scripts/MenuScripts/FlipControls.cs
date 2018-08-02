using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FlipControls : MonoBehaviour
{
    [SerializeField] private PlayerColors player;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(FlipPlayerControls);
    }

    /// <summary>
    /// Does the actual flippin'
    /// </summary>
    public void FlipPlayerControls()
    {
        ChooseControls.PlayerControls controls = ChooseControls.controls[player];
        KeyCode leftKey = controls.leftKey;
        controls.leftKey = controls.rightKey;
        controls.rightKey = leftKey;

        ChooseControls.UpdatePlayerControlText(player);
    }
}