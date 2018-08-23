using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueRenumeration : MonoBehaviour
{
    [SerializeField] protected Text text;

    [SerializeField] private float minVal, maxVal;

    private float value;

    protected float RenumerateWithClamp(float renumerationValue)
    {
        return value = Mathf.Clamp(value + renumerationValue, minVal, maxVal);
    }
}