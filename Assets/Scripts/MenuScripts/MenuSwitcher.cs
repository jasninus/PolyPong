using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwitcher : MonoBehaviour
{
    protected static MenuSwitcher currentMenu;

    [SerializeField] private Vector3 disabledPosition, enabledPosition;

    [SerializeField] private float menuEndPositionMargin, smoothTime;
    private float currentSpeed;

    public void SwitchToThisMenu()
    {
        // If this is the current menu, do nothing
        if (this == currentMenu) { return; }

        // Stop previous running coroutines
        currentMenu.StopAllCoroutines();
        StopAllCoroutines();

        currentMenu.StartMove(disabledPosition);
        StartMove(enabledPosition);

        currentMenu = this;
    }

    protected void StartMove(Vector3 target)
    {
        StartCoroutine(MoveMenu(target));
    }

    private IEnumerator MoveMenu(Vector3 target)
    {
        var velocity = Vector3.zero;

        while (Vector3.Distance(transform.position, target) > menuEndPositionMargin)
        {
            ApplySmoothDamp(target, ref velocity);
            yield return null;
        }
    }

    private void ApplySmoothDamp(Vector3 position, ref Vector3 velocity)
    {
        transform.position = Vector3.SmoothDamp(transform.position, position, ref velocity, smoothTime);
    }
}