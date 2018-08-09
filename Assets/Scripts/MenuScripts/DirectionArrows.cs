using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArrows : MonoBehaviour
{
    [SerializeField] private GameObject prefabArrow;

    [HideInInspector] public Transform currentArrow;

    private const float disFromPlayerEdge = 0.6f;
    private float playerEdgeDisFromMiddle;

    public void AttachLeftArrow(Player player)
    {
        if (!player)
            return;

        if (playerEdgeDisFromMiddle == 0)
            playerEdgeDisFromMiddle = player.transform.GetChild(0).localScale.x / 2;

        Transform pTrans = player.transform.GetChild(0);

        currentArrow = Instantiate(prefabArrow, pTrans).transform;
        currentArrow.localPosition = new Vector3(-(playerEdgeDisFromMiddle + disFromPlayerEdge), 0, 0);
        currentArrow.localScale = new Vector3(0.4f, 4, 1);
        currentArrow.Rotate(0, 0, 180);
    }

    public void SwitchArrowDirection()
    {
        if (currentArrow)
        {
            currentArrow.localPosition = new Vector3(playerEdgeDisFromMiddle + disFromPlayerEdge, 0, 0);
            currentArrow.Rotate(0, 0, 180);
        }
    }

    public void RemoveArrow()
    {
        if (currentArrow)
            Destroy(currentArrow.gameObject);
    }
}