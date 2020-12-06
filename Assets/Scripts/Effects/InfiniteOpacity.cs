using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteOpacity : MonoBehaviour
{
    public float time;
    public Vector2 range;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color color = this.GetComponent<SpriteRenderer>().color;
        color.a = PostProcessingControls.sinEasingLoop(Time.time, range.x, range.y, 0.4F);
        this.GetComponent<SpriteRenderer>().color = color;
    }
}
