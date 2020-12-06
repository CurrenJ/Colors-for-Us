using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunbeamSpawner : MonoBehaviour
{

    public GameObject sunbeamSpawnable;
    public int minSpawnWait;
    public int maxSpawnWait;
    public int curSpawnWait;
    public float lastSpawnTime;
    public float xVariation;
    public float yPlacement;
    public float rotVariation;
    public float minScale;
    public float maxScale;
    public float minOpacity;
    public float maxOpacity;

    // Start is called before the first frame update
    void Start()
    {
        lastSpawnTime = Time.time;
        curSpawnWait = Random.Range(minSpawnWait, maxSpawnWait);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastSpawnTime >= curSpawnWait)
        {
            spawn(xVariation, yPlacement);
        }
    }

    private void spawn(float xVariation, float  yPlacement) {
        GameObject beam = Instantiate(sunbeamSpawnable);
        beam.transform.SetParent(this.transform);
        beam.transform.localPosition = new Vector3(Random.Range(-xVariation, xVariation), yPlacement, 0);
        beam.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-rotVariation, rotVariation));
        float scale = Random.Range(minScale, maxScale);
        beam.transform.localScale = new Vector3(scale, scale, 1);
        beam.GetComponent<SunbeanSpawnable>().setMaxIntensity(Random.Range(minOpacity, maxOpacity));

        lastSpawnTime = Time.time;
        curSpawnWait = Random.Range(minSpawnWait, maxSpawnWait);
    }
}
