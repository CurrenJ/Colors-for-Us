 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingControls : MonoBehaviour
{
    public float minCA;
    public float maxCA;
    public float frequencyCA;
    public float minB;
    public float maxB;
    public float frequencyB;
    public bool disableChromAbOnMobile;

    Bloom bloomLayer = null;
    ChromaticAberration chromaticAberrationLayer = null;
    public bool disableCA;

    // Start is called before the first frame update
    void Start()
    {
        // somewhere during initializing
        Volume volume = gameObject.GetComponent<Volume>();
        volume.profile.TryGet<Bloom>(out bloomLayer);
        volume.profile.TryGet<ChromaticAberration>(out chromaticAberrationLayer);

        if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && disableChromAbOnMobile)
            disableCA = true;

        if (disableCA) {
            volume.profile.Remove<ChromaticAberration>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!disableCA)
            chromaticAberrationLayer.intensity.value = sinEasingLoop(Time.time, minCA, maxCA, frequencyCA);

        bloomLayer.intensity.value = sinEasingLoop(Time.time, minB, maxB, frequencyB);
    }

    public static float sinEasingLoop(float time, float min, float max, float frequency) {
        float t = (Time.time % (1 / frequency));
        float dif = Mathf.Abs(max - min);
        return (-dif / 2) * Mathf.Cos(2 * Mathf.PI * frequency * t) + dif / 2 + min;
    }
}
