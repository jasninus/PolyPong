using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToggleObjOnOff : MonoBehaviour
{
    [SerializeField] private GameObject toToggle;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleObj);
    }

    public void ToggleObj()
    {
        toToggle.SetActive(!toToggle.activeInHierarchy);
    }
}