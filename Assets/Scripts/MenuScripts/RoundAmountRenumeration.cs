using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundAmountRenumeration : ValueRenumeration
{
    public static int selectedRoundAmount;

    public void RenumerateRoundAmount(float renumerationValue)
    {
        text.text = RenumerateWithClamp(renumerationValue).ToString();

        selectedRoundAmount = int.Parse(text.text);
    }
}