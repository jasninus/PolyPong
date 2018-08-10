using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static bool GetValidInput(ref KeyCode validKey)
    {
        int length = Enum.GetValues(typeof(KeyCode)).Length;

        for (int i = 0; i < length; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
            {
                validKey = (KeyCode)i;
                return true;
            }
        }

        return false;
    }
}