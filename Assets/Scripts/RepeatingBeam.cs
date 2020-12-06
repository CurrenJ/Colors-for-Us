using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class RepeatingBeam : MonoBehaviour
{
    public GameObject beamPrefab;
    private float sizeY;
    private List<GameObject> sprites;
    public float cycleTime;
    public float length;
    public Color beamColor;
    private Vector3 startPos;
        
    // Start is called before the first frame update
    void Start()
    {
        sprites = new List<GameObject>();
        sizeY = beamPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        startPos = this.transform.localPosition;

        for (int b = 0; b < length; b++) {
            GameObject beam = createBeamGO();
            Vector3 pos = beam.transform.localPosition;
            pos.y = -sizeY + sizeY * b;
            beam.transform.localPosition = pos;
            beam.GetComponent<SpriteRenderer>().color = beamColor;
            beam.GetComponent<Light2D>().color = beamColor;
            sprites.Add(beam);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float scalar = (Time.time % cycleTime) / cycleTime;

        Vector3 newPos = startPos;
        newPos.y = startPos.y + sizeY * scalar;
        this.transform.localPosition = newPos;
    }

    private GameObject createBeamGO() {
        GameObject b = Instantiate(beamPrefab);
        b.transform.SetParent(this.transform);
        return b;
    }
}
