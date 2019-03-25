using UnityEngine;

public class PsychedelicSizePowerup : PowerupBase
{
    [SerializeField] private float targetMagnitude, angleSpeed, sinRotMagnitude;
    private float angle, currentMagnitude;

    private InGameManager inGameManager;

    [SerializeField] /*TODO delete serialization*/private bool isActivated, endedEarly;

    [SerializeField] private float w1, w2, w3, w4, w5, w6;

    private void Awake()
    {
        inGameManager = GameObject.FindWithTag("LevelHandler").GetComponent<InGameManager>();
    }

    protected override void SpecialActivate()
    {
        isActivated = true;
        LevelManager.innerLerpTo = LevelManager.innerPoints;
        LevelManager.outerLerpTo = LevelManager.outerPoints;
    }

    protected override void Revert()
    {
        isActivated = false;

        if (!endedEarly)
        {
            inGameManager.ReturnToNormalLevel();
        }
    }

    private void Update()
    {
        if (!isActivated)
        {
            return;
        }

        if (!InGameManager.shouldLerpSmaller && !InGameManager.shouldLerpToCircle)
        {
            inGameManager.UpdatePsychedelicLevel(sinRotMagnitude, GetWeightedRadians(LevelManager.innerPoints.Count), currentMagnitude, angle);
        }
        else
        {
            endedEarly = true;
            isActivated = false;
        }

        angle += angle + angleSpeed * Time.deltaTime > Mathf.PI * 2 ? -angle : angleSpeed * Time.deltaTime;
    }

    public float[] GetWeightedRadians(int cornerAmount)
    {
        if (currentMagnitude < targetMagnitude)
        {
            currentMagnitude += targetMagnitude * Time.deltaTime;
        }

        const float radians = 2 * Mathf.PI;

        float[] weights = new float[cornerAmount];

        for (int i = 0; i < cornerAmount; i++)
        {
            float compareVal = (radians / cornerAmount) * i;

            float diff = Mathf.Abs(angle - compareVal) <= Mathf.PI ?
                Mathf.Abs(angle - compareVal) :
                radians - Mathf.Abs(angle - compareVal);

            float normalizedDiff = ((-2 * diff + Mathf.PI) / Mathf.PI) * currentMagnitude;

            weights[i] = normalizedDiff;
        }

        return weights;
    }
}