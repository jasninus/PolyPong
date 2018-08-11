using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArrows : MonoBehaviour
{
    [SerializeField] private Transform arrow;

    private SpriteRenderer arrowRend;

    private void Awake()
    {
        arrowRend = arrow.GetComponent<SpriteRenderer>();
    }

    private const float disFromPlayerEdge = 0.8f;
    private float playerEdgeDisFromMiddle;

    public void AttachLeftArrow(Player player)
    {
        if (!player)
            return;

        arrowRend.enabled = true;
        arrow.parent = player.transform.GetChild(0);
        arrow.localPosition = new Vector3(-(playerEdgeDisFromMiddle + disFromPlayerEdge), 0, 0);

        arrow.localScale = new Vector3(0.5f, 5, 1);
        arrow.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void SwitchArrowDirection()
    {
        arrow.localPosition = new Vector3(playerEdgeDisFromMiddle + disFromPlayerEdge, 0, 0);
        arrow.Rotate(0, 0, 180);
    }

    public void FlipArrow()
    {
        arrow.Rotate(0, 0, 180);
    }

    public void HideArrow()
    {
        arrow.parent = null;
        arrowRend.enabled = false;
    }
}