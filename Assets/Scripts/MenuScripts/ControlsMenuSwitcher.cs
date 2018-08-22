using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenuSwitcher : MenuSwitcher
{
    public bool IsCurrentMenu => currentMenu == this;

    private void Awake()
    {
        currentMenu = this;
    }
}