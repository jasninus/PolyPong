using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class PsychadelicSizePowerup : PowerupBase
{
    [SerializeField] private float targetMagnitude, angleSpeed, sinRotMagnitude;
    private float angle, currentMagnitude;

    private LevelPoints lerpManager;

    private InGameManager _inGameManager;

    private float startTime;

    private bool isActivated, endedEarly;

    private void Awake()
    {
        _inGameManager = GameObject.FindWithTag("LevelHandler").GetComponent<InGameManager>();
        lerpManager = _inGameManager.GetComponent<LevelPoints>();
    }

    protected override void SpecialActivate()
    {
        startTime = Time.time;
        isActivated = true;
        InGameManager.innerLerpTo = InGameManager.innerPoints;
        InGameManager.outerLerpTo = InGameManager.outerPoints;
    }

    protected override void Revert()
    {
        isActivated = false;

        if (!endedEarly)
            _inGameManager.ReturnToNormalLevel();
    }

    private void Update()
    {
        if (!isActivated)
            return;

        if (!InGameManager.shouldLerpSmaller && !InGameManager.shouldLerpToCircle)
        {
            _inGameManager.UpdatePsychadelicLevel(sinRotMagnitude, GetWeightedRadians(InGameManager.innerPoints.Count), currentMagnitude, angle);
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
            currentMagnitude += targetMagnitude * Time.deltaTime;

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