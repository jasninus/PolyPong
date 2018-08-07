using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManagerTest : MonoBehaviour
{
    private KeyCode key;

    private void Start()
    {
        float ticks = Environment.TickCount;

        for (int i = 0; i < 1000; i++)
        {
            GetValidInput(ref key);
        }

        Debug.Log(Environment.TickCount - ticks);
    }

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