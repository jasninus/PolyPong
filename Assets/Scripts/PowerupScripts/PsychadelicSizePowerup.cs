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

    private LevelManager levelManager;

    private float startTime;

    private bool isActivated, endedEarly;

    private void Awake()
    {
        levelManager = GameObject.FindWithTag("LevelHandler").GetComponent<LevelManager>();
        lerpManager = levelManager.GetComponent<LevelPoints>();
    }

    protected override void SpecialActivate()
    {
        startTime = Time.time;
        isActivated = true;
        LevelManager.innerLerpTo = LevelManager.innerPoints;
        LevelManager.outerLerpTo = LevelManager.outerPoints;
    }

    protected override void Revert()
    {
        isActivated = false;

        if (!endedEarly)
            levelManager.ReturnToNormalLevel();
    }

    private void Update()
    {
        if (!isActivated)
            return;

        if (!LevelManager.shouldLerpSmaller && !LevelManager.shouldLerpToCircle)
        {
            levelManager.UpdatePsychadelicLevel(sinRotMagnitude, GetWeightedRadians(LevelManager.innerPoints.Count), currentMagnitude, angle);
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