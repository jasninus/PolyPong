using UnityEngine;

public class SelectionBall : MonoBehaviour
{
    private Vector3 target;

    [SerializeField] private float rotationSpeed, sizeChangeMultiplier, sizeChangeSpeed, moveSpeed;

    private void FixedUpdate()
    {
        // Rotate
        transform.Rotate(0, 0, rotationSpeed);

        // Scale
        transform.localScale *= 1 + Mathf.Sin(Time.time * sizeChangeSpeed) * sizeChangeMultiplier;
    }

    public void LerpPosToPoint()
    {
        transform.position = Vector3.Lerp(transform.position, target, moveSpeed);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            CancelInvoke("LerpPosToPoint");
        }
    }

    public void StartPosLerpToPoint(Vector3 target)
    {
        this.target = target;
        InvokeRepeating("LerpPosToPoint", 0, 0.02f);
    }
}