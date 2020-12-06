using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SunbeanSpawnable : MonoBehaviour
{
    public int maxDecayTime;
    public int minDecayTime;
    private float decayTime;
    private float spawnTime;
    public float maxIntensity;

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Time.time;
        decayTime = Random.Range(minDecayTime, maxDecayTime);
        updateIntensity();
    }

    // Update is called once per frame
    void Update()
    {
        updateIntensity();
    }

    private void updateIntensity() {
        float normalScalar = 1 - ((Time.time - spawnTime) / decayTime);
        if (normalScalar <= 0)
            Destroy(this.gameObject);
        this.GetComponentInChildren<Light2D>().intensity = easingFunction(normalScalar);
    }

    private float easingFunction(float normalIn) {
        return -4 * maxIntensity * Mathf.Pow(normalIn - 0.5F, 2) + maxIntensity;
    }

    public void setMaxIntensity(float maxIntensity) {
        this.maxIntensity = maxIntensity;
    }
}
