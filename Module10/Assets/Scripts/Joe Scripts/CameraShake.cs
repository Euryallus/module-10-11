using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraShakeType
{
    StopAfterTime,
    ReduceOverTime
}

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform targetCameraTransform;

    private float           shakeIntensity;
    private float           startShakeIntensity;
    private float           shakeFrequency;
    private float           shakeSpeed;
    private float           totalShakeTime;
    private CameraShakeType shakeType;

    private float           shakeTimer;
    private float           frequencyTimer;
    private Vector3         basePosition;
    private Vector3         targetOffset;

    private bool            shaking;

    private const float DefaultShakeIntensity   = 0.05f;
    private const float DefaultShakeFrequency   = 0.03f;
    private const float DefaultShakeSpeed       = 0.3f;

    private void Start()
    {
        frequencyTimer = shakeFrequency;
        basePosition = targetCameraTransform.localPosition;
    }

    void Update()
    {
        if(shaking)
        {
            shakeTimer      += Time.deltaTime;
            frequencyTimer  += Time.deltaTime;

            targetCameraTransform.localPosition = Vector3.Lerp(targetCameraTransform.localPosition, basePosition + targetOffset, shakeSpeed);

            if(shakeType == CameraShakeType.ReduceOverTime)
            {
                shakeIntensity = Mathf.Lerp(startShakeIntensity, 0.0f, shakeTimer / totalShakeTime);
            }

            if(shakeTimer > totalShakeTime)
            {
                targetCameraTransform.localPosition = basePosition;
                shaking = false;
            }
            else
            {
                if (frequencyTimer > shakeFrequency)
                {
                    targetOffset = new Vector3(Random.Range(-shakeIntensity, shakeIntensity), Random.Range(-shakeIntensity, shakeIntensity), Random.Range(-shakeIntensity, shakeIntensity));

                    frequencyTimer = 0.0f;
                }
            }
        }
    }

    public void ShakeCameraForTime(float time, CameraShakeType type,
                                    float intensity = DefaultShakeIntensity, float frequency = DefaultShakeFrequency, float speed = DefaultShakeSpeed)
    {
        totalShakeTime      = time;
        shakeIntensity      = intensity;
        startShakeIntensity = intensity;
        shakeFrequency      = frequency;
        shakeSpeed          = speed;

        shakeType           = type;

        shakeTimer          = 0.0f;
        frequencyTimer      = 0.0f;

        shaking = true;
    }
}