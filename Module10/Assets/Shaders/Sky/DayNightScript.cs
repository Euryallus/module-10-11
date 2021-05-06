using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightScript : MonoBehaviour
{

    [SerializeField]
    private GameObject SunObject;
    private Light SunLight;

    private int environLightIntensity;

    [SerializeField]
    private float timeOfDay = 0;

    [SerializeField]
    private float timeProgression = 50f;

    [SerializeField]
    Material skyboxMat;

    Vector4 cloudOffset;

    [SerializeField]
    private float cloudScrollSpeed = 0.05f;

    [SerializeField]
    private float fogDensityNight = 0.2f;
    [SerializeField]
    private float fogDensityDay = 0.005f;
    [SerializeField]
    private float ambientLightNight = 0.5f;

    [SerializeField]
    private Color dayFogColour;
    [SerializeField]
    private Color nightFogColour;

    

    // Start is called before the first frame update
    void Start()
    {
        SunObject.transform.rotation = Quaternion.Euler(90, 0, 0);

        timeOfDay = 12f;

        cloudOffset = new Vector4(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        SunObject.transform.Rotate(new Vector3(Time.deltaTime * timeProgression, 0, 0));

        timeOfDay += (Time.deltaTime * timeProgression) / 360f * 24;

        if(timeOfDay >= 24.0f)
        {
            timeOfDay = 0.0f;
        }
        //timeOfDay = (timeOfDay / 360f) * 24f;

        if(timeOfDay > 20.0f)
        {
            RenderSettings.ambientIntensity =   Mathf.Lerp(RenderSettings.ambientIntensity, ambientLightNight, Time.deltaTime * (1/ timeProgression));
            RenderSettings.fogDensity =         Mathf.Lerp(RenderSettings.fogDensity, fogDensityNight, Time.deltaTime * (1 / timeProgression));
            RenderSettings.fogColor =           Color.Lerp(RenderSettings.fogColor, nightFogColour, Time.deltaTime * (1 / timeProgression));
        }
        if(timeOfDay < 6.0f)
        {
            RenderSettings.ambientIntensity =   Mathf.Lerp(RenderSettings.ambientIntensity, 1.0f, Time.deltaTime * (1 / timeProgression));
            RenderSettings.fogDensity =         Mathf.Lerp(RenderSettings.fogDensity, fogDensityDay, Time.deltaTime * (1 / timeProgression));
            RenderSettings.fogColor =           Color.Lerp(RenderSettings.fogColor, dayFogColour, Time.deltaTime * (1 / timeProgression));
        }

        if(timeOfDay > 6.0f && timeOfDay < 20.0f)
        {
            RenderSettings.ambientIntensity =   1.0f;
            RenderSettings.fogDensity =         fogDensityDay;
            RenderSettings.fogColor =           dayFogColour;
        }

        cloudOffset.y += Time.deltaTime * cloudScrollSpeed;

        skyboxMat.SetVector("_CloudOffset", cloudOffset);

    }
}
