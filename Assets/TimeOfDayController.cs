using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TimeOfDayController : MonoBehaviour
{
    public GameObject sun;
    public Vector2 dawnPosition;
    public Vector2 duskPosition;
    public Vector2 noonPosition;
    public GameObject lightGO;
    public Gradient lightColor;
    public GameObject sky;
    public Gradient skyColor;

    //1 = 1 min per second
    public float timeMultiplier;
    public float time;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        updateTime();
    }

    private void updateTime()
    {
        time += Time.deltaTime * timeMultiplier;
        if (time <= 720)
        {
            sun.transform.localPosition = Vector3.Lerp(dawnPosition, noonPosition, (float)easingFunction(time / 720));
        }
        else if (time > 720)
        {
            sun.transform.localPosition = Vector3.Lerp(noonPosition, duskPosition, (float)easingFunction((time - 720) / 720));
            if (time > 1440)
            {
                time %= 1440;
            }
        }

        if (sky != null)
            setSkyColors(skyColor.Evaluate(time / 1440), skyColor.Evaluate(Mathf.Clamp(time - 120, 0, 1440) / 1440));

        if (lightGO != null)
        {
            setLightColor(lightColor.Evaluate(time / 1440));
            lightGO.GetComponent<Light2D>().intensity = -0.5F * Mathf.Abs((time - 720) / 720) + 1;
        }
    }

    private void setSkyColors(Color colorA, Color colorB)
    {
        sky.GetComponent<Renderer>().material.SetColor("_ColorA", colorA);
        sky.GetComponent<Renderer>().material.SetColor("_ColorB", colorB);
    }

    private void setLightColor(Color color)
    {
        lightGO.GetComponent<Light2D>().color = color;
    }

    public static double easingFunction(float currentTime) {
        return -Mathf.Pow(currentTime-1, 2) + 1;
    }
}
