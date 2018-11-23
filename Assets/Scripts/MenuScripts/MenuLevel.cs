using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLevel : MonoBehaviour
{
    private PointLerp lerper;

    private void Awake()
    {
        lerper = new PointLerp();
    }
}