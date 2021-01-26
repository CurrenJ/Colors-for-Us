using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Range(0.0F, 1.0F)]
    public float scrollSpeed;
    public Vector2 center;
    private Vector2 start;
    public bool lockYAxis;

    void Start() {
        this.start = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 camPos = Camera.main.transform.position;
        Vector2 newPos = this.transform.localPosition = (camPos - center) * (scrollSpeed);
        if (lockYAxis)
            this.transform.localPosition = new Vector2(newPos.x, start.y);
        else this.transform.localPosition = newPos;
    }
}
