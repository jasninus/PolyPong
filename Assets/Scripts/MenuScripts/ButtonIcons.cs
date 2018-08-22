using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonIcons : MonoBehaviour
{
    [SerializeField] private Sprite[] replacementSprites;

    public void SetButtonIcon(KeyCode key, Text text, Image image)
    {
        string rString = null;
        Sprite rSprite = null;

        switch (key) // Here will all the replacements go. For replacing with text assign rString, for replacing with sprite assign rSprite
        {
            case KeyCode.DownArrow:
                rString = "Down";
                break;

            case KeyCode.Comma:
                rSprite = replacementSprites[0];
                break;
        }

        if (rString != null)
        {
            text.text = rString;
            image.gameObject.SetActive(false);
            return;
        }

        if (rSprite != null)
        {
            image.sprite = rSprite;
            return;
        }

        text.text = key.ToString();
        image.gameObject.SetActive(false);
    }
}